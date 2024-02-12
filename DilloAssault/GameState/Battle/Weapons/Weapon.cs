using DilloAssault.Configuration;
using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Effects;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.GameState.Battle.Weapons
{
    public class Weapon(WeaponJson weaponJson)
    {
        private readonly int FireRate = weaponJson.FireRate;

        public WeaponType Type { get; set; } = weaponJson.Type;
        public int AmmoInClip { get; set; } = weaponJson.ClipSize;
        public int Ammo { get; set; } = weaponJson.ClipSize * (weaponJson.ClipsGiven - 1);
        public int FramesSinceFired { get; set; } = -1;

        public void Update()
        {
            if (FramesSinceFired >= 0 && FramesSinceFired < 1000)
            {
                FramesSinceFired++;
            }
        }

        public void Reload()
        {
            var clipSize = weaponJson.ClipSize;
            if (Ammo >= clipSize - AmmoInClip)
            {
                Ammo -= clipSize - AmmoInClip;
                AmmoInClip = clipSize;
            }
        }

        public bool CanFire()
        {
            return (FramesSinceFired < 0 || FramesSinceFired > FireRate) && AmmoInClip > 0;
        }

        public void Fire(Vector2 weaponTip, double weaponAngle, Direction direction)
        {
            if (!CanFire())
            {
                throw new InvalidOperationException();
            }

            FramesSinceFired = 0;

            var configuration = ConfigurationManager.GetWeaponConfiguration(Type);

            var newAngle = weaponAngle;

            for (int i = 0; i < configuration.BulletsFired; i++) {
                Random r = new();
                double rDouble = r.NextDouble() * configuration.AccuracyConeDegrees - configuration.AccuracyConeDegrees / 2;

                BulletManager.CreateBullet(configuration, weaponTip, (float)(newAngle + rDouble * Math.PI / 180));
            }

            AmmoInClip--;

            var effectType = ConfigurationManager.GetWeaponConfiguration(Type).EffectType;

            EffectManager.CreateEffect(weaponTip, Enum.Parse<EffectType>(effectType), direction, weaponAngle);
        }
    }
}
