using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Update
{
    public class CrateUpdate
    {
        [JsonProperty("Ts")]
        public List<CrateType> NewTypes { get; set; }

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> NewXs { get; set; }

        [JsonProperty("Ys")]
        public List<int> NewFinalYs { get; set; }

        [JsonProperty("Gs")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> NewGoingDowns { get; set; }

        [JsonProperty("Ds")]
        public List<int> DeletedIds { get; set; }
    }
}
