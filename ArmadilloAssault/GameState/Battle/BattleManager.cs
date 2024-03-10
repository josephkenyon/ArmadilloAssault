using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static BattleFrame BattleFrame { get; set; }
        private static Menu PauseMenu { get; set; }
        private static bool ServerPaused { get; set; }
        private static Battle Battle { get; set; }

        public static bool Paused => Battle == null || Battle.Paused;
        public static bool GameOver => Battle == null || Battle.ModeManager.GameOver;

        public static void Initialize(string data)
        {
            Battle = new(data);
        }

        public static void Initialize(Dictionary<PlayerIndex, AvatarType> avatars, string data)
        {
            Battle = new(avatars, data);
        }

        public static void UpdateServer()
        {
            if (ControlsManager.IsControlDownStart(0, Control.Start) && (!Battle.ModeManager.GameOver))
            {
                SetPaused(!Battle.Paused);
            }

            if (Battle.Paused)
            {
                PauseMenu.Update();

                if (ControlsManager.IsControlDownStart(0, Control.Confirm))
                {
                    var button = PauseMenu.Buttons.FirstOrDefault(button => button.Selected);

                    if (button != null)
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => MenuManager.InvokeAction(action, button.Data));
                    }
                }

                if (!Battle.ModeManager.GameOver)
                {
                    return;
                }
            }

            if (Battle.ModeManager.GameOver && !Battle.Paused)
            {
                SetPaused(true);
            }

            Battle?.Update();
        }

        public static void UpdateClient()
        {
            if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                SetPaused(!Battle.Paused);
            }

            if (Battle.Paused)
            {
                PauseMenu.Update();

                if (ControlsManager.IsControlDownStart(0, Control.Confirm))
                {
                    var button = PauseMenu.Buttons.FirstOrDefault(button => button.Selected);

                    if (button != null)
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => MenuManager.InvokeAction(action, button.Data));
                    }
                }

                return;
            }

            if (Battle.Paused)
            {
                PauseMenu.Update();
            }

            if (!ServerPaused)
            {
                Battle.Update();
            }
        }

        public static void Draw()
        {
            Battle?.Draw();

            if (Battle.Paused)
            {
                DrawingManager.DrawMenuButtons(PauseMenu.VisibleButtons);
                return;
            }
        }

        public static void SetPaused(bool paused, bool enforcedByServer = false)
        {
            Battle.Paused = paused;
            if (Battle.Paused)
            {
                var menuName = Battle.ModeManager.GameOver ? "Game_Over" : "Pause";
                PauseMenu = new Menu(ConfigurationManager.GetMenuConfiguration(menuName), true);
            }

            if (ServerManager.IsServing)
            {
                ServerManager.BroadcastPause(paused);
            }
            else if (ClientManager.IsActive)
            {
                if (!enforcedByServer)
                {
                    _ = ClientManager.BroadcastPausedChange(paused);
                }
                else if (!Battle.ModeManager.GameOver)
                {
                    ServerPaused = paused;
                }
            }
        }

        public static void SetGameOver()
        {
            Battle?.ModeManager?.OverrideGameOver();
        }

        public static void PlayerDisconnected(int index)
        {
            if (Battle != null)
            {
                Battle.Avatars.Remove((PlayerIndex)index);
            }
        }
    }
}
