using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Update
{
    public class CrateUpdate
    {
        [JsonProperty("I")]
        public List<int> NewIds { get; set; }

        [JsonProperty("T")]
        public List<CrateType> NewTypes { get; set; }

        [JsonProperty("X")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> NewXs { get; set; }

        [JsonProperty("Y")]
        public List<int> NewFinalYs { get; set; }

        [JsonProperty("G")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> NewGoingDowns { get; set; }

        [JsonProperty("D")]
        public List<int> DeletedIds { get; set; }
    }
}
