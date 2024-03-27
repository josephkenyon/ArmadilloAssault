using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BulletFrame
    {
        [JsonProperty("WTs")]
        public List<WeaponType> WeaponTypes { get; set; } = [];

        [JsonProperty("Rs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Rotations { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Xs { get; set; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Ys { get; set; } = [];
    }
}
