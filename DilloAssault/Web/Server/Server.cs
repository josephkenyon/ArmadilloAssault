using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState.Battle.Players;
using DilloAssault.Web.Communication;
using DilloAssault.Web.Communication.Frame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DilloAssault.Web.Server
{
    public class Server : IObserver<string>
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

        public void MessageIntialization()
        {
            var index = 1;
            Players.ForEach(player =>
            {
                var message = new ServerMessage {
                    Type = ServerMessageType.BattleInitialization,
                    ClientId = player.ConnectionId,
                    PlayerCount = Players.Count + 1,
                    AvatarIndex = index++,
                    BattleFrame = BattleManager.BattleFrame
                };

                Broadcast(message);
            });
        }

        public void MessageGameEnd()
        {
            Players.ForEach(player =>
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleTermination,
                    ClientId = player.ConnectionId
                };
                Broadcast(message);
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
                    ClientId = player.ConnectionId,
                    BattleFrame = battleFrame
                };

                Broadcast(message);

                index++;
            });
        }

        public void TerminateGame()
        {
            GameStateManager.State = State.Menu;
        }

        public void Broadcast(ServerMessage serverMessage)
        {
            var message = JsonConvert.SerializeObject(serverMessage);
            WebSocketServer.WebSocketServices["/game"].Sessions.Broadcast(message);
        }

        public void OnNext(string value)
        {
            try
            {
                var clientMessage = JsonConvert.DeserializeObject<ClientMessage>(value);

                if (clientMessage.Type == ClientMessageType.JoinGame)
                {
                    OnJoinGame(clientMessage.Name);
                }
                else if (clientMessage.Type == ClientMessageType.InputUpdate)
                {
                    //Trace.WriteLine($"Received input at frame {BattleManager.BattleFrameCounter}");
                    UpdateInput(clientMessage);
                }
                else if (clientMessage.Type == ClientMessageType.LeaveGame)
                {
                    Players.RemoveAll(player => player.ConnectionId == clientMessage.Id);

                    var message = new ServerMessage
                    {
                        Type = ServerMessageType.EndConnection,
                        ClientId = clientMessage.Id
                    };

                    Broadcast(message);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void OnJoinGame(string name)
        {
            if (!Players.Any(player => player.Name == name))
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "");
                GuidString = GuidString.Replace("+", "");

                Players.Add(new Player
                {
                    Name = name,
                    ConnectionId = GuidString,
                    PlayerIndex = Players.Count
                });

                var initiation = new ServerMessage { Type = ServerMessageType.Initiate, ClientId = GuidString, Name = name };

                Broadcast(initiation);
            }
        }

        private void UpdateInput(ClientMessage message)
        {
            if (message.Id == null)
            {
                throw new Exception("Client id was null.");
            }

            var index = Players.FindIndex(player => player.ConnectionId == message.Id);
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

        public class Game : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                ServerManager.Observer.OnNext(e.Data);
            }
        }
    }
}
