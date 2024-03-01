using ArmadilloAssault.Configuration.Weapons;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public class Bullet
    {
        public WeaponType WeaponType { get; set; }
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        public float DamageModifier { get; set; }

        public void Update()
        {
            Position = new Vector2(
                Position.X + (float)(BulletManager.Bullet_Speed * Math.Cos(Angle)),
                Position.Y + (float)(BulletManager.Bullet_Speed * Math.Sin(Angle))
            );
        }
    }
}
