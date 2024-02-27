using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Menu.Lobby;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmadilloAssault.GameState.Menu
{
    public static class MenuManager
    {
        private static Stack<string> MenuStack { get; set; } = new Stack<string>(["Root"]);
        private static Scene Scene { get; set; }

        private static Assets.Menu CurrentMenu { get; set; }

        public static Color BackgroundColor { get; private set; } = new Color(95, 77, 170);
        public static Color ForegroundColor { get; private set; } = new Color(209, 123, 20);

        public static Point ButtonSize { get; private set; } = new Point(384, 96);

        public static LobbyFrame LobbyFrame { get; set; }
        public static LobbyState LobbyState { get; set; }

        public static void Initialize()
        {
            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(SceneName.gusty_gorge.ToString());
            Scene = new Scene(sceneConfiguration);

            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly);

            UpdateCurrentMenu();
        }

        public static void Update()
        {
            CurrentMenu.Update();

            CloudManager.UpdateClouds();

            foreach (var button in CurrentMenu.Buttons)
            {
                var rectangle = button.GetRectangle();
                button.Enabled = ConditionFulfilled(button.EnabledCondition);
                button.Selected = button.Enabled && rectangle.Contains(ControlsManager.GetAimPosition(0));
            }

            if (ControlsManager.IsControlDownStart(0, Control.Confirm))
            {
                var button = CurrentMenu.Buttons.FirstOrDefault(button => button.Selected);

                if (button != null)
                {
                    SoundManager.PlayMenuSound(MenuSound.confirm);
                    button.Actions.ForEach(async action => await InvokeAction(action, button.Data));
                }
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                var cancelButton = CurrentMenu.Buttons.SingleOrDefault(button =>
                    button.Actions.Contains(MenuAction.back)
                    || button.Actions.Contains(MenuAction.stop_client)
                );

                SoundManager.PlayMenuSound(MenuSound.cancel);

                if (cancelButton != null)
                {
                    cancelButton.Actions.ForEach(async action => await InvokeAction(action, cancelButton.Data));
                }
                else
                {
                    Back();
                }
            }
            else if (LobbyState != null)
            {
                LobbyState.Update();
                LobbyFrame = LobbyState.CreateFrame();

                if (ServerManager.IsServing)
                {
                    ServerManager.SendLobbyFrame(LobbyFrame);
                }
            }
        }

        private static Task InvokeAction(MenuAction action, string data)
        {
            switch (action)
            {
                case MenuAction.navigate_to:
                    MenuStack.Push(data);
                    UpdateCurrentMenu();
                    break;
                case MenuAction.start_client:
                    ClientManager.AttemptConnection();
                    break;
                case MenuAction.stop_client:
                    ClientManager.TerminateConnection();
                    break;
                case MenuAction.start_server:
                    ServerManager.StartServer();
                    break;
                case MenuAction.stop_server:
                    ServerManager.TerminateServer();
                    break;
                case MenuAction.start_game:
                    ServerManager.StartGame(data);
                    break;
                case MenuAction.open_editor:
                    GameStateManager.State = State.Editor;
                    break;
                case MenuAction.select_avatar:
                    _ = SelectAvatar(data);
                    break;
                case MenuAction.back:
                    Back();
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private static async Task SelectAvatar(string avatarTypeString)
        {
            var avatarType = Enum.Parse<AvatarType>(avatarTypeString);
            if (ServerManager.IsServing)
            {
                LobbyState.AvatarSelected(PlayerIndex.One, avatarType);
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastAvatarSelection(avatarType);
            }
        }

        public static void EnterLobby()
        {
            MenuStack.Pop();
            MenuStack.Push("Lobby");
            UpdateCurrentMenu();
        }

        public static void Draw()
        {
            GraphicsManager.Clear(Scene.BackgroundColor);

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f);

            DrawingManager.DrawCollection(CloudManager.Clouds);

            if (MenuStack.Peek() == "Root")
            {
                DrawingManager.DrawTexture(Configuration.Textures.TextureName.logo, new Rectangle(690, 128, 540, 256));
            }

            if (CurrentMenu.LoadingSpinner != null)
            {
                DrawingManager.DrawCollection([CurrentMenu.LoadingSpinner]);
            }

            DrawingManager.DrawMenuButtons(CurrentMenu.Buttons);

            if (MenuStack.Peek() == "Lobby" && LobbyFrame != null)
            {
                DrawingManager.DrawLobbyPlayerBackgrounds(LobbyFrame.PlayerBackgrounds.Select(rec => rec.ToRectangle));
                DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(LobbyFrame.AvatarFrame));
            }
        }

        private static void UpdateCurrentMenu() {
            CurrentMenu = new Assets.Menu(ConfigurationManager.GetMenuConfiguration(MenuStack.Peek()));

            if (CurrentMenu.Name == "Lobby" && !ClientManager.IsActive)
            {
                LobbyState = new LobbyState();
            }
            else
            {
                LobbyState = null;
            }
        }

        public static void Back()
        {
            MenuStack.Pop();
            if (MenuStack.Count == 0)
            {
                Engine.Quit();
                return;
            }

            UpdateCurrentMenu();
        }

        public static bool ConditionFulfilled(MenuCondition menuCondition)
        {
            return menuCondition switch
            {
                MenuCondition.hosting => !ClientManager.IsActive,
                MenuCondition.not_hosting => ClientManager.IsActive,
                MenuCondition.selection_complete => LobbyState != null && LobbyState.Avatars.Count == ServerManager.PlayerCount,
                _ => true
            };
        }

        public static void UpdateAvatarSelection(int index, AvatarType avatarType)
        {
            LobbyState?.AvatarSelected((PlayerIndex)index, avatarType);
        }

        public static void PlayerDisconnected(int index)
        {
            LobbyState?.AvatarDisconnected((PlayerIndex)index);
        }
    }
}
