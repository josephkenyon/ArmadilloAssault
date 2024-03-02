using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Players;
using ArmadilloAssault.GameState.Menu;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication;
using ArmadilloAssault.Web.Communication.Frame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ArmadilloAssault.Web.Server
{
    public class Server
    {
        public List<Player> Players { get; set; }
        private WebSocketServer WebSocketServer { get; set; }

        private IEnumerable<Player> ClientPlayers => Players.Where(player => player.ConnectionId != null);

        public void Start()
        {
            Players = [
                new Player
                {
                    PlayerIndex = 0
                }
            ];

            WebSocketServer = new WebSocketServer(25565);
            WebSocketServer.AddWebSocketService<Game>("/game");
            WebSocketServer.Start();
        }

        public void MessageIntialization(string data)
        {
            foreach (var player in ClientPlayers)
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleInitialization,
                    AvatarTypes = MenuManager.LobbyState.Avatars.Values.Select(avatar => avatar.Type).ToList(),
                    SceneName = data,
                    BattleFrame = BattleManager.BattleFrame
                };

                Broadcast(message, player.ConnectionId);
            }
        }

        public void MessageGameEnd()
        {
            foreach (var player in ClientPlayers)
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleTermination,
                };

                Broadcast(message, player.ConnectionId);
            }
        }

        public void SendBattleFrame(BattleFrame battleFrame, IEnumerable<HudFrame> hudFrames)
        {
            foreach (var player in ClientPlayers)
            {
                var index = battleFrame.AvatarFrame.PlayerIndices.FindIndex(pIndex => pIndex == player.PlayerIndex);

                battleFrame.HudFrame = hudFrames.ElementAt(index);

                var colorIndex = 0;
                battleFrame.AvatarFrame.Colors.ForEach(color =>
                {
                    color.A = MathUtils.GetAlpha(color, colorIndex++, index);
                });

                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleUpdate,
                    BattleFrame = battleFrame
                };

                Broadcast(message, player.ConnectionId);
            }
        }

        public void SendLobbyFrame(LobbyFrame lobbyFrame)
        {
            var index = 1;
            foreach (var player in ClientPlayers)
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.LobbyUpdate,
                    LobbyFrame = lobbyFrame
                };

                Broadcast(message, player.ConnectionId);

                index++;
            }
        }

        public void Broadcast(ServerMessage serverMessage, string id)
        {
            var message = JsonConvert.SerializeObject(serverMessage);
            WebSocketServer.WebSocketServices["/game"].Sessions.SendTo(message, id);
        }

        public void OnNext(string value, string id)
        {
            try
            {
                var clientMessage = JsonConvert.DeserializeObject<ClientMessage>(value);

                if (clientMessage.Type == ClientMessageType.JoinGame)
                {
                    OnJoinGame(clientMessage.Name, id);
                }
                else if (clientMessage.Type == ClientMessageType.InputUpdate)
                {
                    UpdateInput(clientMessage, id);
                }
                else if (clientMessage.Type == ClientMessageType.AvatarSelection)
                {
                    UpdateAvatarSelection(clientMessage, id);
                }
                else if (clientMessage.Type == ClientMessageType.NextLevel)
                {
                    _ = MenuManager.NextLevel();
                }
                else if (clientMessage.Type == ClientMessageType.PreviousLevel)
                {
                    _ = MenuManager.PreviousLevel();
                }
                else if (clientMessage.Type == ClientMessageType.Pause)
                {
                    BattleManager.SetPaused(clientMessage.Paused);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void OnJoinGame(string name, string id)
        {
            if (GameStateManager.State == State.Menu)
            {
                var newIndex = 0;
                while (Players.Any(player => player.PlayerIndex == newIndex))
                {
                    newIndex++;
                }

                Players.Add(new Player
                {
                    Name = name,
                    ConnectionId = id,
                    PlayerIndex = newIndex
                });

                Players = [.. Players.OrderBy(player => player.PlayerIndex)];
            }
            else
            {
                try
                {
                    WebSocketServer.WebSocketServices["/game"].Sessions.CloseSession(id);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void UpdateInput(ClientMessage message, string id)
        {
            var index = Players.FindIndex(player => player.ConnectionId == id);
            if (index != -1)
            {
                Players[index].AreControlsDown = message.AreControlsDown;
                Players[index].AimPosition = new Microsoft.Xna.Framework.Vector2(message.AimX, message.AimY);
            }
        }

        private void UpdateAvatarSelection(ClientMessage message, string id)
        {
            var player = Players.Find(player => player.ConnectionId == id);
            if (player != null)
            {
                MenuManager.UpdateAvatarSelection(player.PlayerIndex, message.AvatarType);
            }
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void Stop()
        {
            WebSocketServer.Stop();
            WebSocketServer = null;
        }

        public void ClientDisconnected(string id)
        {
            var player = Players.FirstOrDefault(player => player.ConnectionId == id);

            if (player != null)
            {
                Players.Remove(player);
                ServerManager.PlayerDisconnected(player.PlayerIndex);
            }
        }

        public void BroadcastPause(bool paused)
        {
            foreach (var player in ClientPlayers)
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.Pause,
                    Paused = paused
                };

                Broadcast(message, player.ConnectionId);
            }
        }

        public class Game : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                ServerManager.ClientMessage(e.Data, ID);
            }

            protected override void OnClose(CloseEventArgs e)
            {
                ServerManager.ClientDisconnected(ID);
            }
        }
    }
}
