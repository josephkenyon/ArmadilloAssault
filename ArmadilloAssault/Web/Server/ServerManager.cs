using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Players;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArmadilloAssault.Web.Server
{
    public static class ServerManager
    {
        private static Server Server { get; set; }
        public static int PlayerCount => Server != null && Server.Players != null ? Server.Players.Count : 0;
        public static List<int> PlayerIndices => Server != null && Server.Players != null ? Server.Players.Select(player => player.PlayerIndex).ToList() : [];

        private static Player GetPlayer(int playerIndex) => Server != null && Server.Players != null ? Server.Players.Find(player => player.PlayerIndex == playerIndex) : null;

        public static List<Control> GetPlayerControlsDown(int playerIndex)
        {
            var player = GetPlayer(playerIndex);
            if (player != null)
            {
                return player.AreControlsDown;
            }

            return [];
        }

        public static Vector2 GetPlayerAimPosition(int playerIndex)
        {
            var player = GetPlayer(playerIndex);
            if (player != null)
            {
                return player.AimPosition;
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
            var avatarTypeDictionary = new Dictionary<int, AvatarType>();
            var avatarProps = new Dictionary<int, AvatarProp>();

            foreach (var playerIndex in MenuManager.LobbyState.Avatars.Keys) {
                avatarTypeDictionary.Add(playerIndex, MenuManager.LobbyState.Avatars[playerIndex].Type);
                avatarProps.Add(playerIndex, new AvatarProp(MenuManager.LobbyState.Avatars[playerIndex], MenuManager.LobbyState.SelectedMode));
            }

            BattleManager.Initialize(avatarTypeDictionary, MenuManager.LobbyState.PlayerTeamRelations, avatarProps, MenuManager.LobbyState.SelectedMode, data);
            GameStateManager.PushNewState(State.Battle);

            Server.MessageIntialization(BattleManager.BattleStaticData);
        }

        public static void EndGame()
        {
            GameStateManager.PushNewState(State.Menu);

            if (IsServing)
            {
                Server.MessageGameEnd();
            }
        }

        public static void TerminateServer()
        {
            Server.Stop();
            Server = null;
        }

        public static void SendBattleFrame(BattleFrame battleFrame)
        {
            try
            {
                Server.SendBattleFrame(battleFrame);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public static void SendLobbyFrame(LobbyFrame lobbyFrame)
        {
            Server.SendLobbyFrame(lobbyFrame);
        }

        public static void PlayerDisconnected(int index)
        {
            MenuManager.PlayerDisconnected(index);
        }

        public static void ClientDisconnected(string id)
        {
            Server.ClientDisconnected(id);
        }

        public static void ClientMessage(string data, string id)
        {
            Server.OnNext(data, id);
        }

        public static void BroadcastPause(bool paused)
        {
            Server.BroadcastPause(paused);
        }

        public static void BroadcastGameOver()
        {
            Server.BroadcastGameOver();
        }

        public static string GetPlayerName(int playerIndex)
        {
            var player = GetPlayer(playerIndex);
            return player != null && player.Name != null ? player.Name : $"P{playerIndex + 1}";
        }

        public static bool IsServing => Server != null;
    }
}
