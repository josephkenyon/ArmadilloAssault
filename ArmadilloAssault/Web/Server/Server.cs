﻿using ArmadilloAssault.Configuration;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Players;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Web.Communication;
using ArmadilloAssault.Web.Communication.Frame;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
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

        private NetManager UdpServer { get; set; }
        private NetDataWriter UdpWriter { get; set; }

        public void Start()
        {
            Players = [
                new Player
                {
                    PlayerIndex = 0,
                    Name = ConfigurationManager.GetWebJson().Username
                }
            ];

            WebSocketServer = new WebSocketServer(25565);
            WebSocketServer.AddWebSocketService<Game>("/game");
            WebSocketServer.Start();

            var listener = new EventBasedNetListener();
            UdpServer = new NetManager(listener);
            UdpWriter = new();

            UdpServer.Start(25565);

            listener.ConnectionRequestEvent += request =>
            {
                if (UdpServer.ConnectedPeersCount < 6)
                    request.AcceptIfKey("Game");
                else
                    request.Reject();

                Trace.WriteLine("UDP connected");
            };
        }

        public void PollEvents()
        {
            UdpServer.PollEvents();
        }

        public void MessageIntialization(BattleStaticData battleStaticData)
        {
            foreach (var player in ClientPlayers)
            {
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.BattleInitialization,
                    BattleStaticData = battleStaticData,
                    BattleFrame = BattleManager.BattleFrame,
                    PlayerIndex = player.PlayerIndex
                });

                Broadcast(message, player.ConnectionId);
            }
        }

        public void MessageGameEnd()
        {
            if (ClientPlayers.Any())
            {
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.BattleTermination,
                });

                Broadcast(message);
            }
        }

        public void SendBattleFrame(BattleFrame battleFrame)
        {
            if (ClientPlayers.Any())
            {
                var message = JsonConvert.SerializeObject(battleFrame);

                BroadcastUdp(message);
            }
        }

        public void SendBattleUpdate(BattleUpdate battleUpdate)
        {
            if (ClientPlayers.Any())
            {
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.BattleUpdate,
                    BattleUpdate = battleUpdate
                });

                Trace.WriteLine(message);

                Broadcast(message);
            }
        }

        public void SendLobbyFrame(LobbyFrame lobbyFrame)
        {
            if (ClientPlayers.Any())
            {
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.LobbyUpdate,
                    LobbyFrame = lobbyFrame
                });

                Broadcast(message);
            }
        }

        public void Broadcast(string message)
        {
            try
            {
                WebSocketServer.WebSocketServices["/game"].Sessions.Broadcast(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public void Broadcast(string message, string playerId)
        {
            try
            {
                WebSocketServer.WebSocketServices["/game"].Sessions.SendTo(message, playerId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public void BroadcastUdp(string message)
        {
            try
            {
                UdpWriter.Put(message);
                UdpServer.SendToAll(UdpWriter, DeliveryMethod.Sequenced);
                UdpWriter.Reset();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
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
                else if (clientMessage.Type == ClientMessageType.TeamIndexIncrement)
                {
                    _ = MenuManager.IncrementTeamIndex(clientMessage.PlayerIndex);
                }
                else if (clientMessage.Type == ClientMessageType.CrownPlayer)
                {
                    _ = MenuManager.CrownPlayer(clientMessage.PlayerIndex);
                }
                else if (clientMessage.Type == ClientMessageType.NextLevel)
                {
                    _ = MenuManager.NextLevel();
                }
                else if (clientMessage.Type == ClientMessageType.PreviousLevel)
                {
                    _ = MenuManager.PreviousLevel();
                }
                else if (clientMessage.Type == ClientMessageType.NextMode)
                {
                    _ = MenuManager.NextMode();
                }
                else if (clientMessage.Type == ClientMessageType.PreviousMode)
                {
                    _ = MenuManager.PreviousMode();
                }

                else if (clientMessage.Type == ClientMessageType.Pause)
                {
                    BattleManager.ClientPauseRequest(clientMessage.Paused);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void OnJoinGame(string name, string id)
        {
            if (GameStateManager.State == State.Menu && Players.Count < 6)
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

                MenuManager.LobbyState?.AddPlayer(newIndex);

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
                Players[index].AimPosition = new Vector2(message.AimX, message.AimY);
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

            UdpServer?.Stop();
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
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.Pause,
                    Paused = paused
                });

                Broadcast(message, player.ConnectionId);
            }
        }

        public void BroadcastGameOver()
        {
            foreach (var player in ClientPlayers)
            {
                var message = JsonConvert.SerializeObject(new ServerMessage
                {
                    Type = ServerMessageType.GameOver,
                });

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
