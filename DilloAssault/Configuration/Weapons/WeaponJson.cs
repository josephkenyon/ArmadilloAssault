﻿using DilloAssault.Configuration.Generics;
using DilloAssault.Graphics.Drawing.Textures;

namespace DilloAssault.Configuration.Weapons
{
    public class WeaponJson
    {
        public string Type { get; set; }
        public TextureName TextureName { get; set; }
        public string EffectType { get; set; }
        public int ClipSize { get; set; }
        public int MaxRange { get; set; }
        public int FireRate { get; set; }
        public int ReloadRate { get; set; }

        public float BulletsFired { get; set; }
        public float BulletDamage { get; set; }
        public int AccuracyConeDegrees { get; set; }
        public float RecoilStrength { get; set; }
        public int RecoilRecoveryRate { get; set; }

        public PointJson SpriteOffset { get; set; }
    }
}
