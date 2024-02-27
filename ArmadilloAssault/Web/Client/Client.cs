using ArmadilloAssault.Web.Communication;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace ArmadilloAssault.Web.Client
{
    public class Client
    {
        private ClientWebSocket WebSocket { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public bool Connected => WebSocket != null && WebSocket.State == WebSocketState.Open;

        public async Task JoinGame(string ipAddress, string name, CancellationTokenSource cts)
        {
            var serverAddress = $"ws://{ipAddress}:25565/game";

            WebSocket = new();

            CancellationTokenSource = cts;

            try
            {
                await WebSocket.ConnectAsync(new Uri(serverAddress), CancellationTokenSource.Token);

                Trace.WriteLine("Connected to server.");

                Task receivingTask = ReceiveMessages();

                var joinGameMessage = new ClientMessage
                {
                    Type = ClientMessageType.JoinGame,
                    Name = name
                };

                await MessageServer(joinGameMessage);

                ClientManager.ConnectionEstablished();

                await Task.WhenAny(receivingTask);

                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                ClientManager.ConnectionTerminated();
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[4096];

            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                var message = new StringBuilder();

                do
                {
                    result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Trace.WriteLine("Server closed the connection.");
                        return;
                    }

                    message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                MessageReceived(message.ToString());
            }
        }

        public async Task MessageServer(ClientMessage message)
        {
            try
            {
                var value = JsonConvert.SerializeObject(message);

                byte[] buffer = Encoding.UTF8.GetBytes(value);

                await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationTokenSource.Token);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private static void MessageReceived(string messageString)
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
    }
}
