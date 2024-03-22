using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Generics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class ItemFrame
    {
        [JsonProperty("Ts")]
        public List<ItemType> Types { get; set; } = [];

        [JsonProperty("Ds")]
        public List<Direction> Directions { get; set; } = [];

        [JsonProperty("SpXs")]
        public List<int> SpriteXs { get; set; } = [];

        [JsonProperty("TIs")]
        public List<int?> TeamIndices { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Xs { get; set; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Ys { get; set; } = [];
    }
}
