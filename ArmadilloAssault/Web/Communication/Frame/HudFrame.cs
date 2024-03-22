using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class HudFrame
    {
        [JsonProperty("RTs")]
        public List<string> RespawnTimers { get; set; }

        [JsonProperty("Hs")]
        public List<int> Healths { get; set; } = [];

        [JsonProperty("As")]
        public List<int?> Ammos { get; set; } = [];

        [JsonProperty("FTVs")]
        public List<int> FlagTimerValues { get; set; } = [];

        [JsonProperty("FTXs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> FlagTimerXs { get; set; } = [];

        [JsonProperty("FTYs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> FlageTimerYs { get; set; } = [];
    }
}
