using DilloAssault.Configuration;
using DilloAssault.Configuration.Json.Avatars;
using DilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Avatars
{
    public class Avatar(AvatarJson avatarJson)
    {
        public static readonly int spriteWidth = 128;
        public static readonly int spriteHeight = 128;

        public readonly Vector2 MaxVelocity = new(5.0f, 10);

        public readonly float RunningAcceleration = 0.65f;
        public readonly float JumpingAcceleration = 0.5f;

        private Rectangle CollisionBox { get; set; } = ConfigurationHelper.GetRectangle(avatarJson.CollisionBox);
        private IEnumerable<Rectangle> HurtBoxes { get; set; } = ConfigurationHelper.GetHurtBoxes(avatarJson.HurtBoxes);
        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        public float RunningVelocity { get; set; }
        public int InfluenceVelocity { get; set; }
        public int AvailableJumps { get; set; }

        public bool Grounded { get; set; }

        public Rectangle GetCollisionBox()
        {
            if (Direction == Direction.Right)
            {
                return CollisionHelper.OffsetRectangle(CollisionBox, Position);
            }
            else
            {
                return CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(CollisionBox, spriteWidth), Position);
            }
        }
        public IEnumerable<Rectangle> GetHurtBoxes()
        {
            if (Direction == Direction.Right)
            {
                return HurtBoxes.Select(box => CollisionHelper.OffsetRectangle(box, Position));
            }
            else
            {
                return HurtBoxes.Select(box => CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(box, spriteWidth), Position));
            }
        }
    }
}
