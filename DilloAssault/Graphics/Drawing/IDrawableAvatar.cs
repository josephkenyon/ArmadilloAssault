using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.Graphics.Drawing
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
    }
}
