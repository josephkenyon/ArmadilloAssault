using ArmadilloAssault.Web.Communication;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using ArmadilloAssault.Web.Communication.Frame;
using LiteNetLib;

namespace ArmadilloAssault.Web.Client
{
    public class Client
    {
        private ClientWebSocket WebSocket { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public bool Connected => WebSocket != null && WebSocket.State == WebSocketState.Open;

        private NetManager UdpClient = null;

        public async Task JoinGame(string ipAddress, string name, CancellationTokenSource cts)
        {
            var serverAddress = $"ws://{ipAddress}:25565/game";

            WebSocket = new();

            CancellationTokenSource = cts;

            try
            {
                await WebSocket.ConnectAsync(new Uri(serverAddress), CancellationTokenSource.Token);

                var listener = new EventBasedNetListener();
                UdpClient = new NetManager(listener);

                UdpClient.Start();
                UdpClient.Connect(ipAddress, 25565, "Game");
                listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
                {
                    try
                    {
                        var message = dataReader.GetString();
                        BattleFrameReceived(message);
                        dataReader.Recycle();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
               
                };

                Trace.WriteLine("Connected to server.");

                Task tcpReceivingTask = ReceiveMessages();

                var joinGameMessage = new ClientMessage
                {
                    Type = ClientMessageType.JoinGame,
                    Name = name
                };

                await MessageServer(joinGameMessage);

                ClientManager.ConnectionEstablished();

                await Task.WhenAny(tcpReceivingTask);

                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                UdpClient?.Stop();
                ClientManager.ConnectionTerminated();
            }
        }

        public void PollEvents()
        {
            UdpClient.PollEvents();
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[2048];

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

        private static void BattleFrameReceived(string messageString)
        {
            try
            {
                var battleFrame = JsonConvert.DeserializeObject<BattleFrame>(messageString);

                ClientManager.OnServerUpdate(new ServerMessage { Type = ServerMessageType.BattleFrame, BattleFrame = battleFrame });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
