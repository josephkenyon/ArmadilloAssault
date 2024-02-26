using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Menu;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmadilloAssault.Web.Server
{
    public static class ServerManager
    {
        private static Server Server { get; set; }
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

        public static void StartGame(string data)
        {
            BattleManager.Initialize(PlayerCount + 1, data);
            GameStateManager.State = State.Battle;

            Server.MessageIntialization(data);
        }

        public static void EndGame()
        {
            GameStateManager.State = State.Menu;

            Server.MessageGameEnd();
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

        public static void ClientDisconnected(string id)
        {
            Server.ClientDisconnected(id);
        }

        public static void ClientMessage(string data, string id)
        {
            Server.OnNext(data, id);
        }

        public static bool IsServing => Server != null;
    }
}
