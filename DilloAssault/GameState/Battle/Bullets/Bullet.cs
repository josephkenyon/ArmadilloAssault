using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.GameState.Battle.Bullets
{
    public class Bullet
    {
        public TextureName TextureName { get; set; }
        public Point Size { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 DustCloudOffset { get; set; }
        public float Angle { get; set; }
        public int Damage { get; set; }
        public int FrameCounter { get; set; } = 0;
        public int FrameLife { get; set; } = -1;

        public void Update()
        {
            Position = new Vector2(
                Position.X + (float)(BulletManager.Bullet_Speed * Math.Cos(Angle)),
                Position.Y + (float)(BulletManager.Bullet_Speed * Math.Sin(Angle))
            );
        }
    }
}
