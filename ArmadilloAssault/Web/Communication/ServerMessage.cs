using ArmadilloAssault.Web.Communication.Frame;

namespace ArmadilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public ServerMessageType Type { get; set; }
        public string Name { get; set; }
        public string SceneName { get; set; }
        public BattleFrame BattleFrame { get; set; }
        public int AvatarIndex { get; set; }
        public int Hp { get; set; }
        public int Ammo { get; set; }
        public int PlayerCount { get; set; }
    }
}
