﻿using DilloAssault.Configuration.Avatars;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.Web.Communication
{
    public class AvatarUpdate
    {
        public Animation Animation { get; set; }
        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }
        public int AnimationFrame { get; set; }
        public int BreathingFrameCounter { get; set; }
        public float ArmAngle { get; set; }
        public float Recoil { get; set; }
        public float SpinningAngle { get; set; }
    }
}
