using ArmadilloAssault.Web.Communication.Frame;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication
{
    public class ServerMessage
    {
        [JsonProperty("T")]
        public ServerMessageType Type { get; set; }

        [JsonProperty("P")]
        public bool Paused { get; set; }

        [JsonProperty("PI")]
        public int PlayerIndex { get; set; }

        [JsonProperty("BSD")]
        public BattleStaticData BattleStaticData { get; set; }

        [JsonProperty("BF")]
        public BattleFrame BattleFrame { get; set; }

        [JsonProperty("LF")]
        public LobbyFrame LobbyFrame { get; set; }
    }
}
