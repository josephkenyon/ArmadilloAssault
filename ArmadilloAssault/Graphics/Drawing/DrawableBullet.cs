using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Camera;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableBullet(WeaponType type, Vector2 position, float rotation) : IDrawableObject
    {
        public TextureName Texture => ConfigurationManager.GetWeaponConfiguration(type).BulletTexture;
        public float GetRotation() => rotation;
        public Rectangle GetDestinationRectangle() => new(
            position.ToPoint() - CameraManager.Offset,
            ConfigurationManager.GetWeaponConfiguration(type).BulletSize.ToPoint()
        );
    }
}
