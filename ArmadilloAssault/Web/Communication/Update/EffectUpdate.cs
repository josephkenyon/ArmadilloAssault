using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Update
{
    public class EffectUpdate
    {
        [JsonProperty("Ts")]
        public List<EffectType> NewTypes { get; set; }

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> NewXs { get; set; }

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> NewYs { get; set; }

        [JsonProperty("Drs")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> NewDirectionLefts { get; set; }
    }
}
