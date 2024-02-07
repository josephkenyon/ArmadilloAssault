using DilloAssault.Configuration;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.GameState.Battle.Physics;
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
        public int FramesSinceFired { get; set; } = -1;

        public void Update()
        {
            if (FramesSinceFired >= 0 && FramesSinceFired < 1000)
            {
                FramesSinceFired++;
            }
        }

        public bool CanFire()
        {
            return FramesSinceFired < 0 || FramesSinceFired > FireRate;
        }

        public void Fire(Vector2 weaponTip, double weaponAngle, Direction direction)
        {
            if (!CanFire())
            {
                throw new InvalidOperationException();
            }

            FramesSinceFired = 0;

            var configuration = ConfigurationManager.GetWeaponConfiguration(Type.ToString());

            var newAngle = weaponAngle;

            for (int i = 0; i < configuration.BulletsFired; i++) {
                Random r = new();
                double rDouble = r.NextDouble() * configuration.AccuracyConeDegrees - configuration.AccuracyConeDegrees / 2;

                BulletManager.CreateBullet(weaponTip, (float)(newAngle + rDouble * Math.PI / 180));
            }


            var effectType = ConfigurationManager.GetWeaponConfiguration(Type.ToString()).EffectType;

            EffectManager.CreateEffect(weaponTip, Enum.Parse<EffectType>(effectType), direction, weaponAngle);
        }
    }
}
