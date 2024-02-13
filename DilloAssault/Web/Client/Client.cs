
using DilloAssault.Controls;
using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.Generics;
using DilloAssault.Web.Communication;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp;

namespace DilloAssault.Web.Client
{
    public class Client
    {
        private WebSocket WebSocket { get; set; }
        private string Name { get; set; }
        public string ClientId { get; private set; } = null;

        public Vector2 LastAim = Vector2.Zero;
        public bool LastUpdateWasEmpty = false;

        public bool ConnectionClosed = true;
        public bool ActiveConnection => WebSocket != null && !ConnectionClosed;

        public List<AvatarUpdate> AvatarUpdates { get; set; } = [];

        public void JoinGame(string ipAddress, string name)
        {
            WebSocket = new WebSocket($"ws://{ipAddress}:25565/game");

            WebSocket.OnMessage += (sender, e) => MessageReceived(e.Data);

            WebSocket.OnClose += (sender, e) => CloseWebSocket();

            WebSocket.OnError += (sender, e) => CloseWebSocket();

            WebSocket.Connect();
            ConnectionClosed = false;

            Name = name;

            var joinGameMessage = new ClientMessage
            {
                Type = ClientMessageType.JoinGame,
                Name = name
            };

            MessageServer(joinGameMessage);
        }

        private void CloseWebSocket()
        {
            ConnectionClosed = true;
            GameStateManager.State = State.Menu;
        }

        public void MessageInputUpdate()
        {
            var controlsDown = ControlsManager.AreControlsDown(0);
            var aim = ControlsManager.GetAimPosition(0);

            var hasUpdates = controlsDown.Count > 0
                && !MathUtils.FloatsAreEqual(aim.X, LastAim.X)
                && !MathUtils.FloatsAreEqual(aim.Y, LastAim.Y);

            if (hasUpdates || !LastUpdateWasEmpty)
            {
                var clientMessage = new ClientMessage
                {
                    Type = ClientMessageType.InputUpdate,
                    Id = ClientId,
                    AreControlsDown = controlsDown,
                    AimX = aim.X,
                    AimY = aim.Y
                };

                MessageServer(clientMessage);
            }

            LastUpdateWasEmpty = hasUpdates;
            LastAim = aim;
        }

        public void MessageServer(ClientMessage message)
        {
            var value = JsonConvert.SerializeObject(message);

            if (ActiveConnection)
            {
                WebSocket.Send(value);
            }
        }

        private void MessageReceived(string messageString)
        {
            try
            {
                var clientMessage = JsonConvert.DeserializeObject<ServerMessage>(messageString);

                if (clientMessage.Type == ServerMessageType.Initiate && clientMessage.Name == Name)
                {
                    ClientId = clientMessage.ClientId;
                }
                else if (clientMessage.ClientId == ClientId)
                {
                    ProcessUpdate(clientMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ProcessUpdate(ServerMessage serverMessage)
        {
            if (serverMessage.Type == ServerMessageType.BattleInitialization)
            {
                BattleManager.Initialize(serverMessage.PlayerCount);
                GameStateManager.State = State.Battle;
            }
            else if (serverMessage.Type == ServerMessageType.BattleUpdate)
            {
                AvatarUpdates = serverMessage.AvatarUpdates;
            }
        }

        public void Stop()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.LeaveGame,
                Id = ClientId
            };

            MessageServer(message);

            WebSocket.Close();
            WebSocket = null;
        }
    }
}
