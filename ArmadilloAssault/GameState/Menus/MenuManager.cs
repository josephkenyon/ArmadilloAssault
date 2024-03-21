using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Menus.Lobby;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmadilloAssault.GameState.Menus
{
    public static class MenuManager
    {
        private static Stack<string> MenuStack { get; set; } = new Stack<string>(["Root"]);
        private static Scene Scene { get; set; }
        private static CloudManager CloudManager { get; set; }

        private static Menu CurrentMenu { get; set; }

        public static Color DarkBackgroundColor { get; private set; } = new Color(65, 58, 94);
        public static Color BackgroundColor { get; private set; } = new Color(95, 77, 170);
        public static Color ForegroundColor { get; private set; } = new Color(209, 123, 20);

        public static Point ButtonSize { get; private set; } = new Point(384, 96);

        private static LobbyFrame _lobbyFrame;
        public static LobbyFrame LobbyFrame { get { return _lobbyFrame; } set { UpdateLobbyFrame(value); } }

        public static LobbyState LobbyState { get; set; }
        public static Scene PreviewScene { get; set; }

        public static void Initialize()
        {
            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(SceneName.gusty_gorge.ToString());
            Scene = new Scene(sceneConfiguration);

            CloudManager = new(sceneConfiguration.HighCloudsOnly);

            UpdateCurrentMenu();
        }

        public static void Update()
        {
            CurrentMenu.Update();

            CloudManager.UpdateClouds();

            if (ControlsManager.IsControlDownStart(0, Control.Confirm))
            {
                var button = CurrentMenu.Buttons.FirstOrDefault(button => button.Selected);

                if (button != null)
                {
                    SoundManager.PlayMenuSound(MenuSound.confirm);
                    button.Actions.ForEach(async action => await InvokeAction(action, button.Data));
                }
                else if (LobbyFrame != null)
                {
                    var index = 0;
                    foreach (var rec in LobbyFrame.PlayerBackgrounds.Select(background => background.ToRectangle()))
                    {
                        if (rec.Contains(ControlsManager.GetMousePosition(0)))
                        {
                            InvokeAction(MenuAction.increment_team_index, index.ToString());
                        }

                        index++;
                    }

                    index = 0;
                    foreach (var rec in LobbyFrame.PlayerModeButtons.Select(background => background.ToRectangle()))
                    {
                        if (rec.Contains(ControlsManager.GetMousePosition(0)))
                        {
                            InvokeAction(MenuAction.crown_player, index.ToString());
                        }

                        index++;
                    }
                }
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                var cancelButton = CurrentMenu.Buttons.SingleOrDefault(button =>
                    (button.Actions.Contains(MenuAction.back) || button.Actions.Contains(MenuAction.stop_client) || button.Text == "Back") && button.Visible && button.Enabled
                );

                SoundManager.PlayMenuSound(MenuSound.cancel);

                if (cancelButton != null)
                {
                    cancelButton.Actions.ForEach(async action => await InvokeAction(action, cancelButton.Data));
                }
                else if (CurrentMenu.Buttons.Any(button => button.Visible && button.Enabled && button.Actions.Contains(MenuAction.avatar_select)))
                {
                    InvokeAction(MenuAction.avatar_select);
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
            else if (MenuStack.Peek() == "Root" && Keyboard.GetState().IsKeyDown(Keys.Back)) {
                InvokeAction(MenuAction.open_editor);
            }
        }

        public static Task InvokeAction(MenuAction action, string data = "")
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
                    ServerManager.StartGame(LobbyState.SelectedLevel);
                    break;
                case MenuAction.open_tutorial:
                    OpenTutorial();
                    break;
                case MenuAction.open_editor:
                    GameStateManager.PushNewState(State.Editor);
                    break;
                case MenuAction.select_avatar:
                    _ = SelectAvatar(data);
                    break;
                case MenuAction.avatar_select:
                    LobbyState?.SetLevelSelect(false);
                    LobbyState?.SetModeSelect(false);
                    break;
                case MenuAction.increment_team_index:
                    _ = IncrementTeamIndex(int.Parse(data));
                    break;
                case MenuAction.crown_player:
                    _ = CrownPlayer(int.Parse(data));
                    break;
                case MenuAction.mode_select:
                    LobbyState?.SetModeSelect(true);
                    LobbyState?.SetLevelSelect(false);
                    break;
                case MenuAction.level_select:
                    LobbyState?.SetModeSelect(false);
                    LobbyState?.SetLevelSelect(true);
                    break;
                case MenuAction.toggle_fullscreen:
                    GraphicsManager.ToggleFullscreen();
                    break;
                case MenuAction.toggle_music:
                    SoundManager.ToggleMusic();
                    break;
                case MenuAction.toggle_sound_effects:
                    SoundManager.ToggleSoundEffects();
                    break;
                case MenuAction.next_level:
                    _ = NextLevel();
                    break;
                case MenuAction.prev_level:
                    _ = PreviousLevel();
                    break;
                case MenuAction.next_mode:
                    _ = NextMode();
                    break;
                case MenuAction.prev_mode:
                    _ = PreviousMode();
                    break;
                case MenuAction.back:
                    Back();
                    break;
                case MenuAction.end_game:
                    ServerManager.EndGame();
                    break;
                case MenuAction.end_pause:
                    BattleManager.EndPause();
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private static void OpenTutorial()
        {
            var avatarTypeDictionary = new Dictionary<int, AvatarType>
            {
                { 0, AvatarType.Arthur }
            };

            var playerTeamRelations = new Dictionary<int, int>
            {
                { 0, 0 }
            };

            BattleManager.Initialize(avatarTypeDictionary, playerTeamRelations, [], ModeType.Tutorial, "gusty_gorge");
            GameStateManager.PushNewState(State.Battle);
        }

        public static async Task IncrementTeamIndex(int playerIndex)
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.IncrementTeamIndex(playerIndex);
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastTeamIndexIncrement(playerIndex);
            }
        }

        public static async Task CrownPlayer(int playerIndex)
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.CrownPlayer(playerIndex);
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastCrownPlayer(playerIndex);
            }
        }

        private static async Task SelectAvatar(string avatarTypeString)
        {
            var avatarType = Enum.Parse<AvatarType>(avatarTypeString);
            if (ServerManager.IsServing)
            {
                LobbyState.AvatarSelected(0, avatarType);
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
                DrawingManager.DrawTexture(TextureName.logo, new Rectangle(690, 128, 540, 256));
            }

            if (CurrentMenu.LoadingSpinner != null)
            {
                DrawingManager.DrawCollection([CurrentMenu.LoadingSpinner]);
            }

            DrawingManager.DrawMenuButtons(CurrentMenu.Buttons.Where(button => button.Visible));

            if (MenuStack.Peek() == "Lobby" && LobbyFrame != null)
            {
                if (ConditionFulfilled(MenuCondition.avatar_select) || ConditionFulfilled(MenuCondition.mode_select))
                {
                    var playerBackgroundRecs = LobbyFrame.PlayerBackgrounds.Select(rec => rec.ToRectangle());

                    DrawingManager.DrawLobbyPlayerBackgrounds(
                        playerBackgroundRecs,
                        LobbyFrame.PlayerTeamIds,
                        LobbyFrame.PlayerBackgroundIds
                    );

                    DrawingManager.DrawLobbyPlayerNames(
                      playerBackgroundRecs,
                      LobbyFrame.PlayerTeamIds,
                      LobbyFrame.PlayerBackgroundIds
                    );

                    DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(LobbyFrame.AvatarFrame));

                    if (ConditionFulfilled(MenuCondition.mode_select) && ModeType.Regicide == LobbyFrame.SelectedMode)
                    {
                        var recs = LobbyFrame.PlayerModeButtons.Select(rec => rec.ToRectangle());

                        DrawingManager.DrawLobbyPlayerBackgrounds(
                            recs,
                            LobbyFrame.PlayerTeamIds,
                            LobbyFrame.PlayerBackgroundIds
                        );

                        DrawingManager.DrawLobbyPlayerCrowns(
                            recs
                        );
                    }
                }
            }

            if (LobbyFrame != null)
            {
                SoundManager.PlaySounds(LobbyFrame.SoundFrame);
            }

            if (ConditionFulfilled(MenuCondition.level_select) && LobbyFrame != null)
            {
                var sceneJson = ConfigurationManager.GetSceneConfiguration(LobbyFrame.SelectedLevel);

                var backgroundHeight = sceneJson.Size.X == 7680 ? 270 : 540;
                var backgroundOffset = sceneJson.Size.X == 7680 ? 140 : 0;

                DrawingManager.DrawTexture(TextureName.white_pixel, new Rectangle(480, 160 + backgroundOffset, 960, backgroundHeight), color: sceneJson.BackgroundColor != null ? sceneJson.BackgroundColor.ToColor() : Color.CornflowerBlue);

                if (sceneJson.BackBackgroundTexture != TextureName.nothing)
                {
                    DrawingManager.DrawTexture(sceneJson.BackBackgroundTexture, new Rectangle(480, 160 + backgroundOffset, 960, backgroundHeight), 0.75f);
                }

                DrawingManager.DrawTexture(sceneJson.BackgroundTexture, new Rectangle(480, 160 + backgroundOffset, 960, backgroundHeight), color: Color.White * (sceneJson.BackBackgroundTexture != TextureName.nothing ? 1f : 0.75f));

                if (LobbyFrame.SelectedMode == ModeType.King_of_the_Hill && sceneJson.CapturePoint != null)
                {
                    DrawingManager.DrawRectangles([new Rectangle(
                        480 + (sceneJson.CapturePoint.X * LobbyFrame.TileSize),
                        160 + (sceneJson.CapturePoint.Y * LobbyFrame.TileSize) + backgroundOffset,
                        sceneJson.CapturePoint.Width * LobbyFrame.TileSize,
                        sceneJson.CapturePoint.Height * LobbyFrame.TileSize
                    )], Color.White);
                }

                if (PreviewScene != null)
                {
                    foreach (var list in PreviewScene.TileLists)
                    {
                        DrawingManager.DrawCollection([.. list.Tiles]);
                    }
                }
            }
            else if (ConditionFulfilled(MenuCondition.mode_select) && LobbyFrame != null)
            {
                var toolTip = ConfigurationManager.GetToolTip(LobbyFrame.SelectedMode.ToString());

                DrawingManager.DrawTooltip(ConfigurationManager.GetToolTip(LobbyFrame.SelectedMode.ToString()), new Point(960, 288));
            }
        }

        private static void UpdateCurrentMenu() {
            if (MenuStack.Peek() == "Lobby" && ServerManager.IsServing)
            {
                LobbyState ??= new LobbyState();
            }
            else
            {
                LobbyState = null;
            }

            CurrentMenu = new Menu(ConfigurationManager.GetMenuConfiguration(MenuStack.Peek()));
        }

        public static void ClearLobbyFrame()
        {
            _lobbyFrame = null;
        }

        private static void UpdateLobbyFrame(LobbyFrame value)
        {
            var previousLevel = LobbyFrame?.SelectedLevel;

            _lobbyFrame = value;

            if (value.SelectedLevel != previousLevel)
            {
                UpdatePreviewScene();
            }
        }

        private static void UpdatePreviewScene()
        {
            var sceneJson = ConfigurationManager.GetSceneConfiguration(LobbyFrame.SelectedLevel);

            PreviewScene = new Scene(sceneJson);
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

        public static async Task NextLevel()
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.NextLevel();
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastNextLevel();
            }
        }

        public static async Task PreviousLevel()
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.PreviousLevel();
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastPreviousLevel();
            }
        }

        public static async Task NextMode()
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.NextMode();
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastNextMode();
            }
        }

        public static async Task PreviousMode()
        {
            if (ServerManager.IsServing)
            {
                LobbyState?.PreviousMode();
            }
            else if (ClientManager.IsActive)
            {
                await ClientManager.BroadcastPreviousMode();
            }
        }

        public static bool ConditionsFulfilled(List<MenuCondition> menuConditions)
        {
            return menuConditions.All(ConditionFulfilled);
        }

        public static bool ConditionFulfilled(MenuCondition menuCondition)
        {
            return menuCondition switch
            {
                MenuCondition.is_tutorial => ModeType.Tutorial == BattleManager.Mode,
                MenuCondition.hosting => ServerManager.IsServing,
                MenuCondition.being_served => ClientManager.IsActive,
                MenuCondition.avatar_select => LobbyFrame != null && !LobbyFrame.LevelSelect && !LobbyFrame.ModeSelect,
                MenuCondition.level_select => LobbyFrame != null && LobbyFrame.LevelSelect,
                MenuCondition.mode_select => LobbyFrame != null && LobbyFrame.ModeSelect,
                MenuCondition.selection_complete => LobbyState != null && LobbyState.Avatars.Count == ServerManager.PlayerCount,
                MenuCondition.mode_conditions_complete => LobbyState != null && LobbyState.ModeConditionsComplete(),
                _ => true
            };
        }

        private static string UppercaseFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] chars = str.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }

        public static void UpdateAvatarSelection(int index, AvatarType avatarType)
        {
            LobbyState?.AvatarSelected(index, avatarType);
        }

        public static void PlayerDisconnected(int index)
        {
            LobbyState?.AvatarDisconnected(index);
        }

        public static string GetValue(MenuKey menuKey)
        {
            return menuKey switch
            {
                MenuKey.fullscreen_toggle_text => $"Fullscreen: {(GraphicsManager.IsFullscreen ? "Off" : "On")}",
                MenuKey.music_toggle_text => $"Music: {(SoundManager.MusicEnabled ? "Off" : "On")}",
                MenuKey.sound_effects_toggle_text => $"Sound FX: {(SoundManager.SoundEffectsEnabled ? "Off" : "On")}",
                MenuKey.game_mode => LobbyFrame != null ? LobbyFrame.ModeName : "",
                MenuKey.level_name => LobbyFrame != null ? GetFormattedLevelName(LobbyFrame.SelectedLevel) : null,
                _ => null
            };
        }

        private static string GetFormattedLevelName(string value)
        {
            var strings = value.Split("_");
            return string.Join("  ", strings.Select(UppercaseFirst));
        }
    }
}
