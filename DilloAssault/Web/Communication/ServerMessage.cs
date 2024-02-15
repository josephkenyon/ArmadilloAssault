
using DilloAssault.Web.Communication.Updates;

namespace DilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public string ClientId { get; set; }
        public ServerMessageType Type { get; set; }
        public string Name { get; set; }
        public FrameUpdate FrameUpdate { get; set; }
        public int AvatarIndex { get; set; }
        public int PlayerCount { get; set; }
    }
}
