using DilloAssault.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.Web.Server
{
    public static class ServerManager
    {
        private static Server Server { get; set; }
        public static IObserver<string> Observer { get; private set; }
        public static int PlayerCount => Server.Players != null ? Server.Players.Count : 0;

        public static List<Control> GetPlayerControlsDown(int playerIndex)
        {
            if (Server.Players.Count > playerIndex)
            {
                return Server.Players[playerIndex].AreControlsDown;
            }

            return [];
        }

        public static Vector2 GetPlayerAimPosition(int playerIndex)
        {
            if (Server.Players.Count > playerIndex)
            {
                return Server.Players[playerIndex].AimPosition;
            }

            return Vector2.Zero;
        }

        public static void StartServer()
        {
            Server = new Server();

            Observer = Server;

            Server.Start();
        }

        public static void StartGame()
        {
            Server.StartGame();
        }

        public static void TerminateGame()
        {
            Server.TerminateGame();
        }

        public static void TerminateServer()
        {
            Server.Stop();
            Server = null;
        }

        public static void SendBattleUpdates()
        {
            Server.SendBattleUpdates();
        }

        public static bool IsServing => Server != null;
    }
}
