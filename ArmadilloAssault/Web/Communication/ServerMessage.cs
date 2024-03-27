using ArmadilloAssault.Web.Communication.Frame;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication
{
    public class ServerMessage
    {
        [JsonProperty("T")]
        public ServerMessageType Type { get; set; }

        [JsonProperty("P", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Paused { get; set; }

        [JsonProperty("PI", NullValueHandling = NullValueHandling.Ignore)]
        public int? PlayerIndex { get; set; }

        [JsonProperty("BSD", NullValueHandling = NullValueHandling.Ignore)]
        public BattleStaticData BattleStaticData { get; set; }

        [JsonProperty("BF", NullValueHandling = NullValueHandling.Ignore)]
        public BattleFrame BattleFrame { get; set; }

        [JsonProperty("BU", NullValueHandling = NullValueHandling.Ignore)]
        public BattleUpdate BattleUpdate { get; set; }

        [JsonProperty("LF", NullValueHandling = NullValueHandling.Ignore)]
        public LobbyFrame LobbyFrame { get; set; }
    }
}
