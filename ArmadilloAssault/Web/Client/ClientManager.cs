using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication;
using ArmadilloAssault.Web.Communication.Frame;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace ArmadilloAssault.Web.Client
{
    public static class ClientManager
    {
        private static Client Client { get; set; }

        public static BattleFrame BattleFrame { get; set; }
        private static CancellationTokenSource CancellationTokenSource { get; set; }
        private static string LastInputFrame { get; set; }

        public static void AttemptConnection()
        {
            if (CancellationTokenSource == null)
            {
                Client = new Client();
                CancellationTokenSource = new();
                _ = Client.JoinGame(ConfigurationManager.GetWebJson().IpAddress ?? "73.216.200.18", ConfigurationManager.GetWebJson().Username, CancellationTokenSource);
            }
        }

        public static void TerminateConnection()
        {
            CancellationTokenSource.Cancel();
        }

        public static void ConnectionTerminated() {
            Client = null;

            GameStateManager.PushNewState(State.Menu);
            MenuManager.ClearLobbyFrame();
            MenuManager.Back();

            CancellationTokenSource = null;
        }

        public static void PollEvents()
        {
            Client?.PollEvents();
        }

        public static void ConnectionEstablished()
        {
            MenuManager.EnterLobby();
        }

        public static async Task BroadcastUpdate()
        {
            if (!Client.UdpEstablished || !BattleManager.SendInputs) return;

            var controlsDown = ControlsManager.AreControlsDown(0);
            var aim = CameraManager.GetAimAngle();

            var inputFrame = new InputFrame
            {
                PlayerIndex = BattleManager.PlayerIndex,
                AreControlsDown = controlsDown.Count > 0 ? controlsDown : null,
                AimX = aim.X,
                AimY = aim.Y
            };

            var message = JsonConvert.SerializeObject(inputFrame);

            if (message != LastInputFrame)
            {
                await Client.MessageInputUpdateUdp(message);
            }

            LastInputFrame = message;
        }

        public static void OnServerUpdate(ServerMessage serverMessage)
        {
            if (serverMessage.Type == ServerMessageType.BattleInitialization)
            {
                BattleManager.Initialize(serverMessage.BattleStaticData, (int)serverMessage.PlayerIndex);
                BattleManager.SetFrame(serverMessage.BattleFrame);

                GameStateManager.PushNewState(State.Battle);
            }
            else if (serverMessage.Type == ServerMessageType.BattleFrame)
            {
                BattleManager.SetFrame(serverMessage.BattleFrame);
            }
            else if (serverMessage.Type == ServerMessageType.BattleUpdate)
            {
                BattleManager.QueueBattleUpdate(serverMessage.BattleUpdate);
            }
            else if (serverMessage.Type == ServerMessageType.LobbyUpdate)
            {
                MenuManager.LobbyFrame = serverMessage.LobbyFrame;
            }
            else if (serverMessage.Type == ServerMessageType.BattleTermination)
            {
                GameStateManager.PushNewState(State.Menu);
            }
            else if (serverMessage.Type == ServerMessageType.Pause && serverMessage.Paused != null)
            {
                BattleManager.SetPaused((bool)serverMessage.Paused);
            }
            else if (serverMessage.Type == ServerMessageType.GameOver)
            {
                BattleManager.SetGameOver();
            }
        }

        public static async Task BroadcastAvatarSelection(AvatarType avatarType)
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.AvatarSelection,
                AvatarType = avatarType
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastNextLevel()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.NextLevel,
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastPreviousLevel()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.PreviousLevel,
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastNextMode()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.NextMode,
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastPreviousMode()
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.PreviousMode,
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastPausedChange(bool paused)
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.Pause,
                Paused = paused,
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastTeamIndexIncrement(int playerIndex)
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.TeamIndexIncrement,
                PlayerIndex = playerIndex
            };

            await Client.MessageServer(message);
        }

        public static async Task BroadcastCrownPlayer(int playerIndex)
        {
            var message = new ClientMessage
            {
                Type = ClientMessageType.CrownPlayer,
                PlayerIndex = playerIndex
            };

            await Client.MessageServer(message);
        }

        public static bool IsActive => Client != null && Client.Connected;
    }
}
