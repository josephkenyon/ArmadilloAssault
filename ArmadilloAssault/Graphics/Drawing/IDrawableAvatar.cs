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
        bool Crowned { get; }
        bool Dead { get; }
        Direction Direction { get; }
        Vector2 Position { get; }
        float Recoil { get; }
        float Rotation { get; }
        bool Spinning { get; }
        TextureName TextureName { get; }
        TextureName WhiteTextureName { get; }
        AvatarType Type { get; }
        TextureName WeaponTexture { get; }
        Color Color { get; set; }
        Color? TeamColor { get; set; }
        float Opacity { get; set; }
    }
}
