using DilloAssault.Configuration;
using DilloAssault.Configuration.Textures;
using DilloAssault.Configuration.Weapons;
using Microsoft.Xna.Framework;

namespace DilloAssault.Graphics.Drawing
{
    public class DrawableBullet(WeaponType type, Vector2 position, float rotation) : IDrawableObject
    {
        public TextureName TextureName => ConfigurationManager.GetWeaponConfiguration(type).BulletTexture;
        public float GetRotation() => rotation;
        public Rectangle GetDestinationRectangle() => new(position.ToPoint(), ConfigurationManager.GetWeaponConfiguration(type).BulletSize.ToPoint());
    }
}
