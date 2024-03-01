using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public interface IDrawableAvatar
    {
        Animation Animation { get; }
        float ArmAngle { get; }
        int AnimationFrame { get; }
        float BreathingYOffset { get; }
        bool Dead { get; }
        Direction Direction { get; }
        Vector2 Position { get; }
        float Recoil { get; }
        float Rotation { get; }
        bool Spinning { get; }
        TextureName TextureName { get; }
        AvatarType Type { get; }
        TextureName WeaponTexture { get; }
        Color Color { get; set; }
        float Opacity { get; set; }
    }
}
