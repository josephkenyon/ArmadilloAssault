using ArmadilloAssault.Sound;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleFrame : ISoundFrameContainer
    {
        [JsonProperty("GOM")]
        public string GameOverMessage { get; set; }
        
        [JsonProperty("AF")]
        public AvatarFrame AvatarFrame { get; set; } = new();
        
        [JsonProperty("BF")]
        public BulletFrame BulletFrame { get; set; }
        
        [JsonProperty("CF")]
        public CrateFrame CrateFrame { get; set; }
        
        [JsonProperty("EF")]
        public EffectFrame EffectFrame { get; set; }
        
        [JsonProperty("HF")]
        public HudFrame HudFrame { get; set; } = new();
        
        [JsonProperty("SF")]
        public SoundFrame SoundFrame { get; set; } = new();

        [JsonProperty("StF")]
        public StatFrame StatFrame { get; set; }
        
        [JsonProperty("MF")]
        public ModeFrame ModeFrame { get; set; } = new();

        [JsonProperty("IF")]
        public ItemFrame ItemFrame { get; set; }
    }
}
