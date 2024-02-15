using DilloAssault.Configuration.Generics;
using DilloAssault.Configuration.Textures;

namespace DilloAssault.Configuration.Weapons
{
    public class WeaponJson
    {
        public WeaponType Type { get; set; }
        public TextureName TextureName { get; set; }
        public string EffectType { get; set; }
        public TextureName BulletTexture { get; set; }
        public PointJson BulletSize { get; set; }
        public int ClipSize { get; set; }
        public int MaxRange { get; set; }
        public int FireRate { get; set; }
        public int ReloadRate { get; set; }

        public float BulletsFired { get; set; }
        public int BulletDamage { get; set; }
        public int AccuracyConeDegrees { get; set; }
        public float RecoilStrength { get; set; }
        public int RecoilRecoveryRate { get; set; }
        public int ClipsGiven { get; set; }

        public PointJson SpriteOffset { get; set; }
    }
}
