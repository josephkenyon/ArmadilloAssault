using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class EffectFrame
    {
        [JsonProperty("Ts")]
        public List<EffectType> Types { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Xs { get; set; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Ys { get; set; } = [];

        [JsonProperty("Ds")]
        public List<Direction?> Directions { get; set; } = [];

        [JsonProperty("Fs")]
        public List<int> Frames { get; set; } = [];
    }
}
