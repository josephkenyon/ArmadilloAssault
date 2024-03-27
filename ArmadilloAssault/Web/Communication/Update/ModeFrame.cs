using ArmadilloAssault.Configuration.Generics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Update
{
    public class ModeFrame
    {
        [JsonProperty("CPC", NullValueHandling = NullValueHandling.Ignore)]
        public ColorJson Colors { get; set; }

        [JsonProperty("CPS", NullValueHandling = NullValueHandling.Ignore)]
        public int? CapturePointSeconds { get; set; }

        [JsonProperty("MVs")]
        public List<int> ModeValues { get; set; } = [];
    }
}
