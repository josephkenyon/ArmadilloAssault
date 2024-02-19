using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.Graphics.Drawing
{
    public class DrawableAvatar : IDrawableAvatar
    {
        public Animation Animation { get; set; }

        public float ArmAngle { get; set; }

        public int AnimationFrame { get; set; }

        public float BreathingYOffset { get; set; }

        public bool Dead { get; set; }

        public Direction Direction { get; set; }

        public Vector2 Position { get; set; }

        public float Recoil { get; set; }

        public float Rotation { get; set; }

        public bool Spinning { get; set; }

        public TextureName TextureName { get; set; }

        public AvatarType Type { get; set; }

        public TextureName WeaponTexture { get; set; }
    }
}
