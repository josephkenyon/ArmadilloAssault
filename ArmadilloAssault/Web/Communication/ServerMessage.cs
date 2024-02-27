using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Web.Communication.Frame;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public ServerMessageType Type { get; set; }
        public string Name { get; set; }
        public string SceneName { get; set; }
        public BattleFrame BattleFrame { get; set; }
        public LobbyFrame LobbyFrame { get; set; }
        public int AvatarIndex { get; set; }
        public int Hp { get; set; }
        public int Ammo { get; set; }
        public List<AvatarType> AvatarTypes { get; set; }
    }
}
