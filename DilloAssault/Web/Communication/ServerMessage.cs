using System.Collections.Generic;

namespace DilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public string ClientId { get; set; }
        public ServerMessageType Type { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public List<AvatarUpdate> AvatarUpdates { get; set; }
    }
}
