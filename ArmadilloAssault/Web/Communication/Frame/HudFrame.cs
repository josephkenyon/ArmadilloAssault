using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class HudFrame
    {
        [JsonProperty("RTs", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RespawnTimers { get; set; }

        [JsonProperty("Hs")]
        public List<int> Healths { get; set; } = [];

        [JsonProperty("As")]
        public List<int?> Ammos { get; set; } = [];

        [JsonProperty("FTVs", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> FlagTimerValues { get; set; }

        [JsonProperty("FTXs", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> FlagTimerXs { get; set; }

        [JsonProperty("FTYs", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> FlageTimerYs { get; set; }
    }
}
