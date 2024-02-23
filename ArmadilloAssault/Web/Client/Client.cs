using ArmadilloAssault.Web.Communication;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using WebSocketSharp;

namespace ArmadilloAssault.Web.Client
{
    public class Client
    {
        private WebSocket WebSocket { get; set; }
        private string Name { get; set; }
        public string ClientId { get; private set; } = null;

        public void JoinGame(string ipAddress, string name)
        {
            try
            {
                Name = name;

                WebSocket = new WebSocket($"ws://{ipAddress}:25565/game");

                WebSocket.OnMessage += (sender, e) => MessageReceived(e.Data);

                WebSocket.OnError += (sender, e) => ClientManager.ConnectionTerminated();

                WebSocket.OnOpen += (sender, e) => {

                    var joinGameMessage = new ClientMessage
                    {
                        Type = ClientMessageType.JoinGame,
                        Name = name
                    };

                    MessageServer(joinGameMessage);
                };

                WebSocket.Connect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                ClientManager.Stop();
            }
        }

        public void MessageServer(ClientMessage message)
        {
            try
            {
                if (ClientId != null)
                {
                    message.Id = ClientId;
                }

                var value = JsonConvert.SerializeObject(message);

                WebSocket.Send(value);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);

                if (!WebSocket.IsAlive)
                {
                    ClientManager.Stop();
                }
            }
        }

        private void MessageReceived(string messageString)
        {
            try
            {
                var clientMessage = JsonConvert.DeserializeObject<ServerMessage>(messageString);

                if (clientMessage.Type == ServerMessageType.Initiate && clientMessage.Name == Name && ClientId == null)
                {
                    ClientId = clientMessage.ClientId;
                }
                else if (clientMessage.ClientId == ClientId)
                {
                    ClientManager.OnServerUpdate(clientMessage);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }


        public void Stop()
        {
            try
            {
                WebSocket.Close();
                WebSocket = null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void MessageEnd()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.LeaveGame,
                Id = ClientId
            };

            MessageServer(message);
        }
    }
}
