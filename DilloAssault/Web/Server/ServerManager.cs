using DilloAssault.Controls;
using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState.Menu;
using DilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DilloAssault.Web.Server
{
    public static class ServerManager
    {
        private static Server Server { get; set; }
        public static IObserver<string> Observer { get; private set; }
        public static int PlayerCount => Server != null && Server.Players != null ? Server.Players.Count : 0;

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

            try
            {
                Server.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Server = null;
                MenuManager.Back();
            }
        }

        public static void StartGame()
        {
            BattleManager.Initialize(PlayerCount + 1);
            GameStateManager.State = State.Battle;

            Server.MessageIntialization();
        }

        public static void EndGame()
        {
            GameStateManager.State = State.Menu;

            Server.MessageGameEnd();
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

        public static void SendBattleFrame(BattleFrame battleFrame, IEnumerable<HudFrame> hudFrames)
        {
            Server.SendBattleUpdates(battleFrame, hudFrames);
        }

        public static bool IsServing => Server != null;
    }
}
