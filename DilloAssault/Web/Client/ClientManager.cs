using DilloAssault.Controls;
using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState.Menu;
using DilloAssault.Generics;
using DilloAssault.Web.Communication;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;

namespace DilloAssault.Web.Client
{
    public static class ClientManager
    {
        private static Client Client { get; set; }
        public static List<AvatarUpdate> AvatarUpdates { get; private set; } = [];

        private static Vector2 LastAim = Vector2.Zero;
        private static bool LastUpdateWasEmpty = false;

        private static Thread Thread { get; set; }

        public static void AttemptConnection()
        {
            Client = new Client();
            Client.JoinGame("73.216.200.18", "Test");
        }

        public static void MessageConnectionEnd()
        {
            Client?.MessageEnd();
        }

        public static void ConnectionEnded()
        {
            GameStateManager.State = State.Menu;
            MenuManager.Back();
        }

        public static void Stop()
        {
            Client.Stop();
            Client = null;
        }

        public static void ConnectionTerminated() {
            GameStateManager.State = State.Menu;
            Client = null;
        }

        public static void BroadcastUpdate()
        {
            if (Client.ClientId != null)
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
        }

        public static void OnServerUpdate(ServerMessage serverMessage)
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
            else if (serverMessage.Type == ServerMessageType.BattleTermination)
            {
                GameStateManager.State = State.Menu;
            }
            else if (serverMessage.Type == ServerMessageType.EndConnection)
            {
                Stop();
                ConnectionEnded();
            }
        }

        public static bool IsActive => Client != null;
    }
}
