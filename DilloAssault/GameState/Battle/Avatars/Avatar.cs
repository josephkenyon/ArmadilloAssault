using DilloAssault.Configuration;
using DilloAssault.Configuration.Json.Avatars;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System;
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

        private readonly Rectangle CollisionBox = ConfigurationHelper.GetRectangle(avatarJson.CollisionBox);

        private readonly IEnumerable<Rectangle> HurtBoxes = ConfigurationHelper.GetHurtBoxes(avatarJson.HurtBoxes);

        private readonly Dictionary<Animation, AnimationJson> Animations = ConfigurationHelper.GetAnimations(avatarJson.Animations);

        public readonly Point Size = new(avatarJson.Size.X, avatarJson.Size.Y);

        private readonly Vector2 ArmOriginJson = new(avatarJson.ArmOrigin.X, avatarJson.ArmOrigin.Y);

        public TextureName SpriteTextureName { get; set; } = avatarJson.SpriteTextureName;
        public TextureName ArmTextureName { get; set; } = avatarJson.ArmTextureName;

        public Animation Animation { get; private set; } = Animation.Resting;

        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        public float RunningVelocity { get; set; }
        public int InfluenceVelocity { get; set; }
        public int AvailableJumps { get; set; }

        public bool RunningBackwards { get; set; }
        public bool Grounded { get; set; }

        public double AimAngle { get; set; }
        public float SpinningAngle { get; private set; }
        public Vector2 AimDirection { get; set; }

        public int FrameCounter { get; set; }
        public int AnimationFrame { get; set; }


        public void Update()
        {
            var animation = Animations[Animation];

            if (FrameCounter >= 3)
            {
                FrameCounter = 0;

                if (RunningBackwards && Animation == Animation.Running)
                {
                    AnimationFrame--;
                }
                else
                {
                    AnimationFrame++;
                }

                if (AnimationFrame == animation.FrameCount)
                {
                    if (Animation == Animation.Running)
                    {
                        AnimationFrame = 0;
                    }
                    else
                    {
                        AnimationFrame--;
                    }
                }
                else if (AnimationFrame <= 0 && RunningBackwards)
                {
                    AnimationFrame = animation.FrameCount - 1;
                }
            }
            else
            {
                FrameCounter++;
            }
        }

        public Vector2 GetArmOrigin() {
            var armOriginX = ArmOriginJson.X;

            if (Direction == Direction.Left)
            {
                armOriginX += ((Size.X / 2) - armOriginX) * 2;
            }

            return new Vector2(armOriginX, ArmOriginJson.Y);
        }


        public Rectangle GetSourceRectangle()
        {
            var animation = Animations[Animation];

            return new Rectangle()
            {
                X = (animation.X + AnimationFrame) * Size.X,
                Y = animation.Y * Size.Y,
                Width = Size.X,
                Height = Size.Y
            };
        }

        public void SetAnimation(Animation animation)
        {
            if (Animation != animation)
            {
                FrameCounter = 0;
                AnimationFrame = 0;

                if (Animation == Animation.Spinning)
                {
                    SpinningAngle = 0f;
                }
            }

            Animation = animation;
        }

        public void IncrementSpin()
        {
            SpinningAngle += 0.1f + (Velocity.Y * 0.004f);
        }

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
