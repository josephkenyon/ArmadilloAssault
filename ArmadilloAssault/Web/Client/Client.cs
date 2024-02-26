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
        public bool Connected => WebSocket != null && WebSocket.ReadyState == WebSocketState.Open;

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

                Trace.WriteLine("Connection finish");

                if (WebSocket.ReadyState == WebSocketState.Open)
                {
                    Trace.WriteLine("Open");
                    ClientManager.ConnectionEstablished();
                }
                else
                {
                    Trace.WriteLine("Close");
                    ClientManager.ConnectionTerminated();
                }
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

                ClientManager.OnServerUpdate(clientMessage);
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
                Type = ClientMessageType.LeaveGame
            };

            MessageServer(message);
        }
    }
}
