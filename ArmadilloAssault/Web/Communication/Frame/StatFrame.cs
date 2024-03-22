using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class StatFrame
    {
        [JsonProperty("PIS")]
        public List<int> PlayerIndices { get; set; } = [];

        [JsonProperty("Ns")]
        public List<string> Names { get; set; } = [];

        [JsonProperty("Ks")]
        public List<int> Kills { get; set; } = [];

        [JsonProperty("Ds")]
        public List<int> Deaths { get; set; } = [];

        [JsonProperty("Dts")]
        public List<int> DamageDealts { get; set; } = [];

        [JsonProperty("Ts")]
        public List<int> DamageTakens { get; set; } = [];
    }
}
