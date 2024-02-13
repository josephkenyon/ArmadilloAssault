using DilloAssault.Controls;
using DilloAssault.Web.Client;
using DilloAssault.Web.Server;
using System;

namespace DilloAssault.GameState.Menu
{
    public static class MenuManager
    {
        public static void Update(Action exit)
        {
            if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                if (ServerManager.IsServing)
                {
                    if (GameStateManager.State == State.Battle)
                    {
                        ServerManager.TerminateGame();
                    }
                    else
                    {
                        ServerManager.TerminateServer();
                        exit.Invoke();
                    }
                }
                else if (ClientManager.IsActive)
                {
                    ClientManager.TerminateConnection();
                }
                else
                {
                    exit.Invoke();
                }

            }
            else if (ControlsManager.IsControlDownStart(0, Control.Right))
            {
                if (ServerManager.IsServing)
                {
                    ServerManager.StartGame();
                }
                else
                {
                    ServerManager.StartServer();
                }
            }
            else if (ControlsManager.IsControlDownStart(0, Control.Left) && !ServerManager.IsServing)
            {
                if (!ClientManager.IsActive)
                {
                    ClientManager.AttemptConnection();
                }
                else if (ClientManager.IsActive)
                {
                    ClientManager.TerminateConnection();
                }
            }
        }
    }
}
