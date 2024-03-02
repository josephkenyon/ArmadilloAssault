using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Web.Communication.Frame;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication
{
    public class ServerMessage
    {
        public ServerMessageType Type { get; set; }
        public bool Paused { get; set; }
        public bool Game_Over { get; set; }
        public string SceneName { get; set; }
        public BattleFrame BattleFrame { get; set; }
        public LobbyFrame LobbyFrame { get; set; }
        public List<AvatarType> AvatarTypes { get; set; }
    }
}
