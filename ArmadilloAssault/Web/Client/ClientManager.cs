using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace ArmadilloAssault.Web.Client
{
    public static class ClientManager
    {
        private static Client Client { get; set; }

        private static Vector2 LastAim = Vector2.Zero;

        private static bool LastUpdateWasEmpty = false;
        public static BattleFrame BattleFrame { get; set; }
        private static CancellationTokenSource CancellationTokenSource { get; set; }

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

        public static void ConnectionEstablished()
        {
            MenuManager.EnterLobby();
        }

        public static async Task BroadcastUpdate()
        {
            var controlsDown = ControlsManager.AreControlsDown(0);
            var aim = CameraManager.GetAimAngle();

            var hasUpdates = controlsDown.Count > 0
                && !MathUtils.FloatsAreEqual(aim.X, LastAim.X)
                && !MathUtils.FloatsAreEqual(aim.Y, LastAim.Y);

            if (hasUpdates || !LastUpdateWasEmpty)
            {
                var clientMessage = new ClientMessage
                {
                    Type = ClientMessageType.InputUpdate,
                    AreControlsDown = controlsDown,
                    AimX = aim.X,
                    AimY = aim.Y
                };

                await Client.MessageServer(clientMessage);
            }

            LastUpdateWasEmpty = hasUpdates;
            LastAim = aim;
        }

        public static void OnServerUpdate(ServerMessage serverMessage)
        {
            if (serverMessage.Type == ServerMessageType.BattleInitialization)
            {
                BattleManager.Initialize(serverMessage.BattleStaticData, serverMessage.PlayerIndex);
                BattleManager.SetFrame(serverMessage.BattleFrame);

                GameStateManager.PushNewState(State.Battle);
            }
            else if (serverMessage.Type == ServerMessageType.BattleUpdate)
            {
                BattleManager.SetFrame(serverMessage.BattleFrame);
            }
            else if (serverMessage.Type == ServerMessageType.LobbyUpdate)
            {
                MenuManager.LobbyFrame = serverMessage.LobbyFrame;
            }
            else if (serverMessage.Type == ServerMessageType.BattleTermination)
            {
                GameStateManager.PushNewState(State.Menu);
            }
            else if (serverMessage.Type == ServerMessageType.Pause)
            {
                BattleManager.SetPaused(serverMessage.Paused);
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
