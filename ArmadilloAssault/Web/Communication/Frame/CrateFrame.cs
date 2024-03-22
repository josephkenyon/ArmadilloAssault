using ArmadilloAssault.GameState.Battle.Crates;
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
        public List<bool> Groundeds { get; set; } = [];

        [JsonProperty("GDs")]
        public List<bool> GoingDowns { get; set; } = [];
    }
}
