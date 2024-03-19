using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableAvatar : IDrawableAvatar
    {
        public Animation Animation { get; set; }
        public float ArmAngle { get; set; }
        public int AnimationFrame { get; set; }
        public float BreathingYOffset { get; set; }
        public bool Crowned { get; set; }
        public bool Dead { get; set; }
        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }
        public float Recoil { get; set; }
        public float Rotation { get; set; }
        public bool Spinning { get; set; }
        public TextureName TextureName { get; set; }
        public TextureName WhiteTextureName { get; set; }
        public AvatarType Type { get; set; }
        public TextureName WeaponTexture { get; set; }
        public Color Color { get; set; }
        public Color? TeamColor { get; set; }
        public float Opacity { get; set; }
    }
}
