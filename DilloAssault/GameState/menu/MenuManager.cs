using DilloAssault.Configuration;
using DilloAssault.Configuration.Menu;
using DilloAssault.Controls;
using DilloAssault.Graphics.Drawing;
using DilloAssault.Web.Client;
using DilloAssault.Web.Server;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Menu
{
    public static class MenuManager
    {
        private static Stack<string> MenuStack { get; set; } = new Stack<string>(["Root"]);

        public static void Update()
        {
            if (ControlsManager.IsControlDownStart(0, Control.Confirm))
            {
                var currentMenu = ConfigurationManager.GetScreenConfiguration(MenuStack.Peek());

                foreach (var button in currentMenu.Buttons)
                {
                    var rectangle = button.GetRectangle();
                    if (rectangle.Contains(ControlsManager.GetAimPosition(0)))
                    {
                        button.Actions.ForEach(action => InvokeAction(action, button.Data));
                        break;
                    }
                }
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                Back();
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
                    ClientManager.MessageConnectionEnd();
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
                case MenuAction.back:
                    Back();
                    break;
                default:
                    break;
            }
        }

        public static void Draw()
        {
            var currentMenu = ConfigurationManager.GetScreenConfiguration(MenuStack.Peek());

            DrawingManager.DrawMenuButtons(currentMenu.Buttons);
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
