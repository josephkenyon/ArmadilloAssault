using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty("X", NullValueHandling = NullValueHandling.Ignore)]
        public float AimX { get; set; }

        [JsonProperty("Y", NullValueHandling = NullValueHandling.Ignore)]
        public float? AimY { get; set; }

        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public List<Control> AreControlsDown { get; set; }

        [JsonProperty("A", NullValueHandling = NullValueHandling.Ignore)]
        public AvatarType? AvatarType { get; set; }

        [JsonProperty("I")]
        public int PlayerIndex { get; set; }
    }
}
