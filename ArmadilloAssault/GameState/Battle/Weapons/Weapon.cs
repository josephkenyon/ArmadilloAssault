using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Weapons
{
    public class Weapon(WeaponJson weaponJson, IWeaponListener weaponListener, bool singleClip = false)
    {
        private readonly int FireRate = weaponJson.FireRate;

        public WeaponType Type { get; set; } = weaponJson.Type;
        public int AmmoInClip { get; set; } = weaponJson.ClipSize;
        public int Ammo { get; set; } = weaponJson.ClipSize * (singleClip ? 1 : (weaponJson.ClipsGiven - 1));
        public int FramesSinceFired { get; set; } = -1;

        private readonly WeaponJson WeaponJson = weaponJson;

        public void Update()
        {
            if (FramesSinceFired >= 0 && FramesSinceFired < 1000)
            {
                FramesSinceFired++;
            }
        }

        public void Reload()
        {
            var clipSize = WeaponJson.ClipSize;
            if (Ammo >= clipSize - AmmoInClip)
            {
                Ammo -= clipSize - AmmoInClip;
                AmmoInClip = clipSize;
            }
            else if (Ammo > 0)
            {
                AmmoInClip += Ammo;
                Ammo = 0;
            }
        }

        public bool HasFullClip()
        {
            return AmmoInClip == WeaponJson.ClipSize;
        }

        public bool CanReload()
        {
            return Ammo > 0;
        }

        public bool CanFire()
        {
            return (FramesSinceFired < 0 || FramesSinceFired > FireRate) && AmmoInClip > 0;
        }

        public void Fire(Vector2 weaponTip, double weaponAngle, Direction direction, float damageModifier, int playerIndex)
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

                weaponListener.CreateBullet(configuration, weaponTip, (float)(newAngle + rDouble * Math.PI / 180), damageModifier, playerIndex);
            }

            AmmoInClip--;

            var effectType = ConfigurationManager.GetWeaponConfiguration(Type).EffectType;

            weaponListener.CreateEffect(weaponTip, Enum.Parse<EffectType>(effectType), direction, weaponAngle);
        }
    }
}
