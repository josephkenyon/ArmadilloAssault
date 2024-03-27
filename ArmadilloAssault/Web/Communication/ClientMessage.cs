using ArmadilloAssault.Configuration.Avatars;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication
{
    public class ClientMessage
    {
        [JsonProperty("T")]
        public ClientMessageType Type { get; set; }

        [JsonProperty("P", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Paused { get; set; }

        [JsonProperty("N", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = null;

        [JsonProperty("A", NullValueHandling = NullValueHandling.Ignore)]
        public AvatarType? AvatarType { get; set; }

        [JsonProperty("I")]
        public int PlayerIndex { get; set; }
    }
}
