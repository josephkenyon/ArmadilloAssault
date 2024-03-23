using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class CrateFrame
    {
        [JsonProperty("Ts")]
        public List<CrateType> Types { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Xs { get; set; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Ys { get; set; } = [];

        [JsonProperty("Gs")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> Groundeds { get; set; } = [];

        [JsonProperty("GDs")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> GoingDowns { get; set; } = [];
    }
}
