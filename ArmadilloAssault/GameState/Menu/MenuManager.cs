using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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
                button.Selected = rectangle.Contains(ControlsManager.GetAimPosition(0));
            }

            if (ControlsManager.IsControlDownStart(0, Control.Confirm))
            {
                var button = CurrentMenu.Buttons.FirstOrDefault(button => button.Selected);

                if (button != null)
                {
                    SoundManager.PlayMenuSound(MenuSound.confirm);
                    button.Actions.ForEach(action => InvokeAction(action, button.Data));
                }
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                var cancelButton = CurrentMenu.Buttons.SingleOrDefault(button => button.Actions.Contains(MenuAction.back));

                SoundManager.PlayMenuSound(MenuSound.cancel);

                if (cancelButton != null)
                {
                    cancelButton.Actions.ForEach(action => InvokeAction(action, cancelButton.Data));
                }
                else
                {
                    Back();
                }
            }
        }

        private static void InvokeAction(MenuAction action, string data)
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
                case MenuAction.back:
                    Back();
                    break;
                default:
                    break;
            }
        }

        public static void EnterClientLobby()
        {
            MenuStack.Pop();
            MenuStack.Push("ClientLobby");
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
        }

        private static void UpdateCurrentMenu() {
            CurrentMenu = new Assets.Menu(ConfigurationManager.GetMenuConfiguration(MenuStack.Peek()));
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
    }
}
