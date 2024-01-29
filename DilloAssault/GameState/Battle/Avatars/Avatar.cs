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

        public readonly Vector2 MaxVelocity = new(8f, 10);

        public readonly float RunningAcceleration = 0.65f;
        public readonly float JumpingAcceleration = 0.5f;
        public readonly float MaxRunningVelocity = 6.5f;

        private readonly Rectangle CollisionBox = ConfigurationHelper.GetRectangle(avatarJson.CollisionBox);

        private readonly Rectangle SpinningCollisionBox = ConfigurationHelper.GetRectangle(avatarJson.SpinningCollisionBox);

        private readonly IEnumerable<Rectangle> HurtBoxes = ConfigurationHelper.GetHurtBoxes(avatarJson.HurtBoxes);

        private readonly Dictionary<Animation, AnimationJson> Animations = ConfigurationHelper.GetAnimations(avatarJson.Animations);

        public readonly Point Size = new(avatarJson.Size.X, avatarJson.Size.Y);

        private readonly Vector2 ArmOriginJson = new(avatarJson.ArmOrigin.X, avatarJson.ArmOrigin.Y);

        public TextureName SpriteTextureName { get; set; } = avatarJson.SpriteTextureName;
        public TextureName ArmTextureName { get; set; } = avatarJson.ArmTextureName;

        public Animation Animation { get; private set; } = Animation.Resting;

        public Direction Direction { get; private set; }
        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        public float RunningVelocity { get; set; }
        public int InfluenceVelocity { get; set; }
        public int AvailableJumps { get; set; }

        public bool RunningBackwards { get; set; }
        public bool Grounded { get; set; }
        public bool IsSpinning => Animation == Animation.Spinning || Animation == Animation.Rolling;

        public double AimAngle { get; set; }
        public float SpinningAngle { get; private set; } = (float)(Math.PI / -2f);
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

                if (!IsSpinning && (Animation == Animation.Spinning || Animation == Animation.Rolling))
                {
                    SpinningAngle = (float)(Math.PI / -2f);
                }
            }

            Animation = animation;
        }

        public void IncrementSpin()
        {
            var velocityX = (Math.Clamp(Velocity.X, -MaxVelocity.X * 2f, MaxVelocity.X * 2f) * 0.007f);
            var velocityY = (Math.Clamp(Velocity.Y, -MaxVelocity.Y * 2f, MaxVelocity.Y * 2f) * 0.007f);

            if (velocityX < 0)
            {
                SpinningAngle += 0.1f - velocityX;
            }
            else
            {
                SpinningAngle += 0.1f + velocityX;
            }

            SpinningAngle += velocityY;
        }

        public Rectangle GetCollisionBox()
        {
            if (Animation == Animation.Spinning || Animation == Animation.Rolling)
            {
                return CollisionHelper.OffsetRectangle(SpinningCollisionBox, Position);
            }

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

        public void SetX(float x)
        {
            Position = new Vector2(x, Position.Y);
            PhysicsManager.MoveIfIntersecting(this, BattleManager.Scene.CollisionBoxes);
        }

        public void SetY(float y)
        {
            Position = new Vector2(Position.X, y);
            PhysicsManager.MoveIfIntersecting(this, BattleManager.Scene.CollisionBoxes);
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
            PhysicsManager.MoveIfIntersecting(this, BattleManager.Scene.CollisionBoxes);
        }

        public void SetDirection(Direction direction)
        {
            if (Direction != direction)
            {
                Direction = direction;
                PhysicsManager.MoveIfIntersecting(this, BattleManager.Scene.CollisionBoxes);
            }
        }
    }
}
