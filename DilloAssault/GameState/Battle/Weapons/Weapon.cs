using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Bullets;
using Microsoft.Xna.Framework;
using System;

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
        }
    }
}
