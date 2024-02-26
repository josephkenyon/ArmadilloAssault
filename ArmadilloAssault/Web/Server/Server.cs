using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Players;
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

        public void Start()
        {
            Players = [];

            WebSocketServer = new WebSocketServer(25565);
            WebSocketServer.AddWebSocketService<Game>("/game");
            WebSocketServer.Start();
        }

        public void MessageIntialization(string data)
        {
            var index = 1;
            Players.ForEach(player =>
            {
                var message = new ServerMessage {
                    Type = ServerMessageType.BattleInitialization,
                    PlayerCount = Players.Count + 1,
                    AvatarIndex = index++,
                    SceneName = data,
                    BattleFrame = BattleManager.BattleFrame
                };

                Broadcast(message, player.ConnectionId);
            });
        }

        public void MessageGameEnd()
        {
            Players.ForEach(player =>
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleTermination,
                };

                Broadcast(message, player.ConnectionId);
            });
        }

        public void SendBattleUpdates(BattleFrame battleFrame, IEnumerable<HudFrame> hudFrames)
        {
            var index = 1;
            Players.ForEach(player =>
            {
                battleFrame.HudFrame = hudFrames.ElementAt(index);

                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleUpdate,
                    BattleFrame = battleFrame
                };

                Broadcast(message, player.ConnectionId);

                index++;
            });
        }

        public void TerminateGame()
        {
            GameStateManager.State = State.Menu;
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
                    //Trace.WriteLine($"Received input at frame {BattleManager.BattleFrameCounter}");
                    UpdateInput(clientMessage, id);
                }
                else if (clientMessage.Type == ClientMessageType.LeaveGame)
                {
                    Players.RemoveAll(player => player.ConnectionId == id);

                    WebSocketServer.WebSocketServices["/game"].Sessions.CloseSession(id);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void OnJoinGame(string name, string id)
        {
            Players.Add(new Player
            {
                Name = name,
                ConnectionId = id,
                PlayerIndex = Players.Count
            });
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

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void ClientDisconnected(string id)
        {
            Players.RemoveAll(player => player.ConnectionId == id);
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
