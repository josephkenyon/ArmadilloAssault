using ArmadilloAssault.Sound;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleFrame : ISoundFrameContainer
    {
        public string GameOverMessage { get; set; }
        public AvatarFrame AvatarFrame { get; set; } = new();
        public BulletFrame BulletFrame { get; set; } = new();
        public CrateFrame CrateFrame { get; set; } = new();
        public EffectFrame EffectFrame { get; set; } = new();
        public HudFrame HudFrame { get; set; } = new();
        public SoundFrame SoundFrame { get; set; } = new();
    }
}
