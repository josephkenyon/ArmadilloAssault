﻿using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Camera;
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
        public static BattleFrame BattleFrame => Battle.Frame;

        public static bool Paused { get; private set; }
        public static bool GameOver => Battle != null && Battle.GameOver;
        public static bool ShowCursor => Paused || (Battle != null && Battle.GameOver);

        public static Mode.ModeType? Mode => Battle?.Mode;

        public static void Initialize(string data, int playerIndex)
        {
            Paused = false;
            Battle = new(data, playerIndex);
        }

        public static void Initialize(Dictionary<int, AvatarType> avatars, Dictionary<int, int> playerTeamRelations, Mode.ModeType mode, string data)
        {
            Paused = false;
            Battle = new(avatars, playerTeamRelations, mode, data);
        }

        public static void Update()
        {
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

        public static void PlayerDisconnected(int index)
        {
            if (Battle != null)
            {
                Battle.Avatars.Remove(index);
            }
        }

        public static void SetFrame(BattleFrame battleFrame)
        {
            Battle.Frame = battleFrame;
        }
    }
}
