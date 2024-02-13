using DilloAssault.GameState;
using DilloAssault.Web.Communication;
using System.Collections.Generic;

namespace DilloAssault.Web.Client
{
    public static class ClientManager
    {
        private static Client Client { get; set; }

        public static void AttemptConnection()
        {
            Client = new Client();
            Client.JoinGame("73.216.200.18", "Test");
        }

        public static void TerminateConnection()
        {
            Client.Stop();
            Client = null;

            GameStateManager.State = State.Menu;
        }

        public static void BroadcastUpdate()
        {
            if (Client.ClientId != null) {
                Client.MessageInputUpdate();
            }
        }

        public static List<AvatarUpdate> AvatarUpdates => Client.AvatarUpdates;

        public static bool IsActive => Client != null && Client.ActiveConnection;
    }
}
