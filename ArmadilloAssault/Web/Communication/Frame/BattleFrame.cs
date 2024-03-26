using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Update;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleFrame
    {
        [JsonProperty("GOM", NullValueHandling = NullValueHandling.Ignore)]
        public string GameOverMessage { get; set; }
        
        [JsonProperty("AF")]
        public AvatarFrame AvatarFrame { get; set; } = new();
        
        [JsonProperty("BF", NullValueHandling = NullValueHandling.Ignore)]
        public BulletFrame BulletFrame { get; set; }
        
        [JsonProperty("CF", NullValueHandling = NullValueHandling.Ignore)]
        public CrateUpdate CrateFrame { get; set; }
        
        [JsonProperty("HF")]
        public HudFrame HudFrame { get; set; } = new();
        
        [JsonProperty("StF", NullValueHandling = NullValueHandling.Ignore)]
        public StatUpdate StatUpdate { get; set; }
        
        [JsonProperty("MF")]
        public ModeUpdate ModeFrame { get; set; } = new();

        [JsonProperty("IF", NullValueHandling = NullValueHandling.Ignore)]
        public ItemFrame ItemFrame { get; set; }
    }
}
