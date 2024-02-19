using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Frame
{
    public class AvatarFrame
    {
        public List<Animation> Animations { get; set; } = [];
        public List<float> ArmAngles { get; set; } = [];
        public List<int> AnimationFrames { get; set; } = [];
        public List<float> BreathingYOffsets { get; set; } = [];
        public List<bool> Deads { get; set; } = [];
        public List<Direction> Directions { get; set; } = [];
        public List<Vector2> Positions { get; } = [];
        public List<float> Recoils { get; set; } = [];
        public List<float> Rotations { get; } = [];
        public List<bool> Spinnings { get; set; } = [];
        public List<TextureName> TextureNames { get; set; } = [];
        public List<AvatarType> Types { get; set; } = [];
        public List<TextureName> WeaponTextures { get; set; } = [];
    }
}
