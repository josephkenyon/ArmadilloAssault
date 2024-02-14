using DilloAssault.GameState.Battle.Players;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using DilloAssault.Web.Communication;
using System.Linq;
using WebSocketSharp.Server;
using WebSocketSharp;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState;
using System.Diagnostics;

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
            Players.ForEach(player =>
            {
                var message = new ServerMessage {
                    Type = ServerMessageType.BattleInitialization,
                    ClientId = player.ConnectionId,
                    PlayerCount = Players.Count + 1
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

        public void SendBattleUpdates()
        {
            var avatarUpdatesList = new List<AvatarUpdate>();

            foreach (var avatar in BattleManager.Avatars.Values)
            {
                var avatarUpdate = new AvatarUpdate
                {
                    Position = avatar.Position,
                    Animation = avatar.Animation,
                    Recoil = avatar.GetRecoil,
                    AnimationFrame = avatar.AnimationFrame,
                    BreathingFrameCounter = avatar.BreathingFrameCounter,
                    Direction = avatar.Direction,
                    SpinningAngle = avatar.SpinningAngle,
                    ArmAngle = (float)avatar.ArmAngle
                };

                avatarUpdatesList.Add(avatarUpdate);
            }

            Players.ForEach(player =>
            {
                var message = new ServerMessage
                {
                    Type = ServerMessageType.BattleUpdate,
                    ClientId = player.ConnectionId,
                    AvatarUpdates = avatarUpdatesList
                };

                Broadcast(message);
            });
        }


        public void TerminateGame()
        {
            GameStateManager.State = State.Menu;
        }

        public void Broadcast(ServerMessage serverMessage)
        {
            WebSocketServer.WebSocketServices["/game"].Sessions.Broadcast(JsonConvert.SerializeObject(serverMessage));
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
