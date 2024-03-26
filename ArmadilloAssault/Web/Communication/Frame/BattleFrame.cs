using ArmadilloAssault.Sound;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleFrame : ISoundFrameContainer
    {
        [JsonProperty("GOM", NullValueHandling = NullValueHandling.Ignore)]
        public string GameOverMessage { get; set; }
        
        [JsonProperty("AF")]
        public AvatarFrame AvatarFrame { get; set; } = new();
        
        [JsonProperty("BF", NullValueHandling = NullValueHandling.Ignore)]
        public BulletFrame BulletFrame { get; set; }
        
        [JsonProperty("CF", NullValueHandling = NullValueHandling.Ignore)]
        public CrateFrame CrateFrame { get; set; }
        
        [JsonProperty("EF", NullValueHandling = NullValueHandling.Ignore)]
        public EffectFrame EffectFrame { get; set; }
        
        [JsonProperty("HF")]
        public HudFrame HudFrame { get; set; } = new();
        
        [JsonProperty("SF", NullValueHandling = NullValueHandling.Ignore)]
        public SoundFrame SoundFrame { get; set; }

        [JsonProperty("StF", NullValueHandling = NullValueHandling.Ignore)]
        public StatFrame StatFrame { get; set; }
        
        [JsonProperty("MF")]
        public ModeFrame ModeFrame { get; set; } = new();

        [JsonProperty("IF", NullValueHandling = NullValueHandling.Ignore)]
        public ItemFrame ItemFrame { get; set; }
    }
}
