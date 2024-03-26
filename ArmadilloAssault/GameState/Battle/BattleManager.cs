using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        private static Menu Menu { get; set; }
        private static Battle Battle { get; set; }
        public static BattleStaticData BattleStaticData => Battle.BattleStaticData;
        public static BattleUpdate BattleUpdate => Battle.BattleUpdate;
        public static BattleFrame BattleFrame => Battle.Frame;

        public static int PlayerIndex { get; private set; }
        public static int FocusPlayerIndex { get; private set; }

        public static bool AmAlive => Battle.IsAlive(PlayerIndex);

        public static bool Paused { get; private set; }
        public static bool GameOver => Battle != null && Battle.GameOver;
        public static bool ShowCursor => Paused || (Battle != null && Battle.GameOver);

        public static ModeType? Mode => Battle?.Mode;

        public static void Initialize(BattleStaticData battleStaticData, int playerIndex)
        {
            PlayerIndex = playerIndex;
            FocusPlayerIndex = playerIndex;

            Paused = false;
            Battle = new(battleStaticData);
        }

        public static void Initialize(Dictionary<int, AvatarType> avatars, Dictionary<int, int> playerTeamRelations, Dictionary<int, AvatarProp> avatarProps, ModeType mode, string data)
        {
            PlayerIndex = 0;
            FocusPlayerIndex = 0;

            Paused = false;
            Battle = new(avatars, playerTeamRelations, avatarProps, mode, data);
        }

        public static void Update()
        {
            if (FocusPlayerIndex != PlayerIndex && AmAlive)
            {
                FocusPlayerIndex = PlayerIndex;
            }

            if (ControlsManager.IsControlDownStart(0, Control.Start) && !Battle.GameOver)
            {
                OnPause();
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Toggle_Scope))
            {
                CameraManager.ToggleScoped();
            }

            if ((Paused || Battle.GameOver) && Menu != null)
            {
                Menu.Update();

                if (ControlsManager.IsControlDownStart(0, Control.Confirm))
                {
                    var button = Menu.Buttons.FirstOrDefault(button => button.Selected);

                    if (button != null)
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => MenuManager.InvokeAction(action, button.Data));
                    }
                }
            }

            if (!Battle.Paused)
            {
                if ((ControlsManager.IsControlDownStart(0, Control.Cycle_Weapon) || ControlsManager.IsControlDownStart(0, Control.Confirm)) && !AmAlive)
                {
                    FocusPlayerIndex = Battle.GetNextPlayerIndex(FocusPlayerIndex);
                }

                Battle?.Update();
            }
        }

        public static void Draw()
        {
            Battle?.Draw();

            if ((Paused || (Battle != null && Battle.GameOver)) && Menu != null)
            {
                DrawingManager.DrawMenuButtons(Menu.VisibleButtons);
                return;
            }
        }

        private static void OnPause(bool? pause = null)
        {
            Paused = pause ?? !Paused;

            if (ServerManager.IsServing || MenuManager.ConditionFulfilled(MenuCondition.is_tutorial))
            {
                Battle.Paused = Paused;

                if (ServerManager.IsServing)
                {
                    ServerManager.BroadcastPause(Paused);
                }
            }
            else if (ClientManager.IsActive)
            {
                _ = ClientManager.BroadcastPausedChange(Paused);
            }

            if (Paused)
            {
                Menu = new Menu(ConfigurationManager.GetMenuConfiguration("Pause"), true);
            }
        }

        public static void EndPause()
        {
            OnPause(false);
        }

        public static void SetPaused(bool paused)
        {
            Paused = paused;
            Battle.Paused = Paused;

            Menu = new Menu(ConfigurationManager.GetMenuConfiguration("Pause"), true);
        }

        public static void ClientPauseRequest(bool paused)
        {
            if (Paused != paused)
            {
                OnPause(paused);
            }
        }

        public static void SetGameOver()
        {
            Battle.GameOver = true;
            Menu = new Menu(ConfigurationManager.GetMenuConfiguration("Game_Over"), true);

            if (ServerManager.IsServing)
            {
                ServerManager.BroadcastGameOver();
            }
        }

        public static void SetFrame(BattleFrame battleFrame)
        {
            Battle.Frame = battleFrame;
        }

        public static void QueueBattleUpdate(BattleUpdate battleUpdate)
        {
            Battle.UpdateQueue.Enqueue(battleUpdate);
        }
    }
}
