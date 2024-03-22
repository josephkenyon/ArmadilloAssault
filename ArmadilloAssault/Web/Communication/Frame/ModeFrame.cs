using ArmadilloAssault.Configuration.Generics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class ModeFrame
    {
        [JsonProperty("CPC")]
        public ColorJson Colors { get; set; }

        [JsonProperty("CPS")]
        public int? CapturePointSeconds { get; set; }

        [JsonProperty("MVs")]
        public List<int> ModeValues { get; set; } = [];
    }
}
