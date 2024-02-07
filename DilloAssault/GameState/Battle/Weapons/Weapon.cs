using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Effects;
using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata;

namespace DilloAssault.GameState.Battle.Weapons
{
    public class Weapon(WeaponJson weaponJson)
    {
        private readonly int FireRate = weaponJson.FireRate;

        public WeaponType Type { get; set; } = Enum.Parse<WeaponType>(weaponJson.Type);
        public int Ammo { get; set; }
        public int FramesSinceFired { get; set; }

        public void Update()
        {
            FramesSinceFired++;
        }

        public bool CanFire()
        {
            return FramesSinceFired > FireRate;
        }

        public void Fire(Vector2 weaponTip, double weaponAngle)
        {
            if (!CanFire())
            {
                throw new InvalidOperationException();
            }

            FramesSinceFired = 0;

            BulletManager.CreateBullet(weaponTip, (float)weaponAngle);

            var effectPosition = new Vector2(
                    weaponTip.X + (float)(18 * Math.Cos(weaponAngle)),
                    weaponTip.Y + (float)(18 * Math.Sin(weaponAngle))
            );

            EffectManager.CreateEffect(effectPosition, EffectType.muzzle_flash_small);
        }
    }
}
