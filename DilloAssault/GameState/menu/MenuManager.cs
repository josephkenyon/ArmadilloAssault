﻿using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Controls;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Server;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Menu
{
    public static class MenuManager
    {
        private static Stack<string> MenuStack { get; set; } = new Stack<string>(["Root"]);

        public static void Update()
        {
            if (ControlsManager.IsControlDownStart(0, Control.Confirm))
            {
                foreach (var button in CurrentMenu.Buttons)
                {
                    var rectangle = button.GetRectangle();
                    if (rectangle.Contains(ControlsManager.GetAimPosition(0)))
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => InvokeAction(action, button.Data));
                        break;
                    }
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
                    break;
                case MenuAction.start_client:
                    ClientManager.AttemptConnection();
                    break;
                case MenuAction.stop_client:
                    if (ClientManager.IsActive)
                    {
                        ClientManager.MessageConnectionEnd();
                    }
                    else
                    {
                        Back();
                    }
                    break;
                case MenuAction.start_server:
                    ServerManager.StartServer();
                    break;
                case MenuAction.stop_server:
                    ServerManager.TerminateServer();
                    break;
                case MenuAction.start_game:
                    ServerManager.StartGame();
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

        private static MenuJson CurrentMenu => ConfigurationManager.GetScreenConfiguration(MenuStack.Peek());

        public static void Draw()
        {
            DrawingManager.DrawMenuButtons(CurrentMenu.Buttons);
        }

        public static void Back()
        {
            MenuStack.Pop();
            if (MenuStack.Count == 0)
            {
                Engine.Quit();
            }
        }
    }
}
