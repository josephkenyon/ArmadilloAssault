using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Menu;
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
            Client = new Client();

            CancellationTokenSource = new();

            var task = Task.Run(() =>
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                Client.JoinGame("73.216.200.18", "Test");
            }, CancellationTokenSource.Token);
        }

        public static void TerminateConnection()
        {
            if (IsActive)
            {
                Client.MessageEnd();
            }
            else
            {
                CancellationTokenSource.Cancel();
            }

            ConnectionTerminated();
        }

        public static void Stop()
        {
            Client.Stop();
            Client = null;
        }

        public static void ConnectionTerminated() {
            Client = null;

            GameStateManager.State = State.Menu;
            MenuManager.Back();
        }

        public static void ConnectionEstablished()
        {
            MenuManager.EnterClientLobby();
        }

        public static void BroadcastUpdate()
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
                    AreControlsDown = controlsDown,
                    AimX = aim.X,
                    AimY = aim.Y
                };

                Client.MessageServer(clientMessage);
            }

            LastUpdateWasEmpty = hasUpdates;
            LastAim = aim;
        }

        public static void OnServerUpdate(ServerMessage serverMessage)
        {
            if (serverMessage.Type == ServerMessageType.BattleInitialization)
            {
                BattleManager.Initialize(serverMessage.PlayerCount, serverMessage.SceneName, serverMessage.AvatarIndex);
                Engine.QueueAction(() => GameStateManager.State = State.Battle);
            }
            else if (serverMessage.Type == ServerMessageType.BattleUpdate)
            {
                BattleManager.BattleFrame = serverMessage.BattleFrame;
            }
        }

        public static bool IsActive => Client != null && Client.Connected;
    }
}
