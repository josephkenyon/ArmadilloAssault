using ArmadilloAssault.Controls;
using ArmadilloAssault.Web.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class InputFrame
    {
        [JsonProperty("I")]
        public int PlayerIndex { get; set; }

        [JsonProperty("X", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FloatConverter))]
        public float? AimX { get; set; }

        [JsonProperty("Y", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FloatConverter))]
        public float? AimY { get; set; }

        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public List<Control> AreControlsDown { get; set; }
    }
}
