using DilloAssault.Web.Communication.Frame;

namespace DilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public string ClientId { get; set; }
        public ServerMessageType Type { get; set; }
        public string Name { get; set; }
        public BattleFrame BattleFrame { get; set; }
        public int AvatarIndex { get; set; }
        public int Hp { get; set; }
        public int Ammo { get; set; }
        public int PlayerCount { get; set; }
    }
}
