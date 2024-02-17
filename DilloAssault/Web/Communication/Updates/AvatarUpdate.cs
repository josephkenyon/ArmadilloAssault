using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Textures;
using DilloAssault.GameState.Battle.Weapons;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public class AvatarUpdate
    {
        public Animation Animation { get; set; }
        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }
        public int Health { get; set; }
        public int AnimationFrame { get; set; }
        public int BreathingFrameCounter { get; set; }
        public float ArmAngle { get; set; }
        public List<Weapon> Weapons { get; set; }
        public int WeaponSelectionIndex { get; set; }
        public float Recoil { get; set; }
        public float SpinningAngle { get; set; }
    }
}
