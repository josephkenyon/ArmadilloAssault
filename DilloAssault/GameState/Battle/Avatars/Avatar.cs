using DilloAssault.Configuration;
using DilloAssault.Configuration.Avatars;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.GameState.Battle.Weapons;
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

        public readonly Vector2 MaxVelocity = new(8f, 11);

        public readonly float RunningAcceleration = 0.65f;
        public readonly float JumpingAcceleration = 0.5f;
        public readonly float MaxRunningVelocity = 6.5f;

        private readonly Rectangle CollisionBox = ConfigurationHelper.GetRectangle(avatarJson.CollisionBox);

        private readonly Rectangle SpinningCollisionBox = ConfigurationHelper.GetRectangle(avatarJson.SpinningCollisionBox);

        private readonly IEnumerable<Rectangle> HurtBoxes = ConfigurationHelper.GetHurtBoxes(avatarJson.HurtBoxes);

        private readonly Dictionary<Animation, AnimationJson> Animations = ConfigurationHelper.GetAnimations(avatarJson.Animations);

        public readonly Point Size = new(avatarJson.Size.X, avatarJson.Size.Y);

        public readonly Point SpriteOffset = new(avatarJson.SpriteOffset.X, avatarJson.SpriteOffset.Y);

        public readonly TextureName SpriteTextureName = avatarJson.SpriteTextureName;
        public readonly TextureName HeadTextureName = avatarJson.HeadTextureName;
        public readonly TextureName LeftArmTextureName = avatarJson.LeftArmTextureName;
        public readonly TextureName RightArmTextureName = avatarJson.RightArmTextureName;

        public Animation Animation { get; private set; } = Animation.Resting;

        public Direction Direction { get; private set; }
        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        public float RunningVelocity { get; set; }
        public int InfluenceVelocity { get; set; }
        public int AvailableJumps { get; set; }

        public bool DropThrough { get; set; }
        public bool RunningBackwards { get; set; }
        public bool Grounded { get; set; }
        public bool CloseToGround { get; set; }
        public bool IsSpinning => Animation == Animation.Spinning || Animation == Animation.Rolling;

        public double ArmAngle { get; set; }
        public float AimAngle { get; set; }

        public float SpinningAngle { get; private set; } = (float)(Math.PI / -2f);
        public Vector2 AimDirection { get; set; }

        private int FrameCounter { get; set; }
        private int AnimationFrame { get; set; }

        private int BreathingFrameCounter { get; set; }
        private bool BreathingIn { get; set; }

        private readonly int BreathingCycleFrameLength = 80;

        private Animation? BufferedAnimation { get; set; }
        private Direction? BufferedDirection { get; set; }

        private List<Weapon> Weapons { get; set; } = [new Weapon(ConfigurationManager.GetWeaponConfiguration(WeaponType.Pistol.ToString()))];
        private int WeaponSelectionIndex { get; set; }

        public Weapon SelectedWeapon => Weapons[WeaponSelectionIndex];

        public bool CanFire => !IsSpinning && Weapons.Count != 0 && Weapons[WeaponSelectionIndex].CanFire();

        public void Update()
        {
            UpdateFrameCounter();
            UpdateBreathingFrameCounter();

            Weapons.ForEach(weapon => weapon.Update());
        }

        private void UpdateFrameCounter()
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

        private void UpdateBreathingFrameCounter()
        {
            if (BreathingFrameCounter == BreathingCycleFrameLength * 2)
            {
                BreathingIn = false;
            }
            else if (BreathingFrameCounter == 0)
            {
                BreathingIn = true;
            }

            if (BreathingIn)
            {
                BreathingFrameCounter++;
            }
            else
            {
                BreathingFrameCounter--;
            }
        }

        public void Fire()
        {
            var weaponTip = GetWeaponTip();

            Weapons[WeaponSelectionIndex].Fire(weaponTip, AimAngle);
        }

        private Vector2 GetWeaponTip()
        {
            var weaponOffset = ConfigurationManager.GetWeaponConfiguration(Weapons[WeaponSelectionIndex].Type.ToString()).SpriteOffset;

            var vectorX = weaponOffset.X;
            var vectorY = weaponOffset.Y;
            if (Direction == Direction.Left)
            {
                vectorY = -vectorY;
            }

            var weaponTipVector = new Vector2(vectorX, vectorY);

            var armOrigin = GetArmOrigin();

            if (Direction == Direction.Right)
            {
                AimAngle = (float)Math.Clamp(AimAngle, -90 * Math.PI / 180, 55 * Math.PI / 180);
            }
            else
            {
                if (AimAngle < 0)
                {
                    AimAngle = (float)Math.Clamp(AimAngle, -270 * Math.PI / 180, -90 * Math.PI / 180);
                }
                else
                {
                    AimAngle = (float)Math.Clamp(AimAngle, 125 * Math.PI / 180, 270 * Math.PI / 180);
                }
            }

            return new Vector2(
                (float)(weaponTipVector.X * Math.Cos(AimAngle) - weaponTipVector.Y * Math.Sin(AimAngle)) + Position.X + armOrigin.X + SpriteOffset.X / 2,
                (float)(weaponTipVector.Y * Math.Cos(AimAngle) + weaponTipVector.X * Math.Sin(AimAngle)) + Position.Y + armOrigin.Y
            );
        }

        public float GetBreathingYOffset()
        {
            var A = 5f;
            var B = BreathingCycleFrameLength * 2;
            var X = Math.Clamp(BreathingFrameCounter, 0, BreathingCycleFrameLength);
            return (float)(A * Math.Sin(X * Math.PI / B)) - 3;
        }

        public Vector2 GetArmOrigin()
        {
            var armOriginX = avatarJson.ArmOrigin.X;
            var armOriginY = avatarJson.ArmOrigin.Y;

            armOriginX += SpriteOffset.X / 2;

            return new Vector2(armOriginX, armOriginY);
        }

        public Vector2 GetArmSpriteOrigin() {
            var armOriginX = avatarJson.ArmOrigin.X;
            var armOriginY = avatarJson.ArmOrigin.Y;

            if (Direction == Direction.Left)
            {
                armOriginX += ((Size.X / 2) - armOriginX) * 2;
            }

            return new Vector2(armOriginX, armOriginY);
        }

        public Vector2 GetHeadOrigin()
        {
            var headOriginX = avatarJson.HeadOrigin.X;

            if (Direction == Direction.Left)
            {
                headOriginX += ((Size.X / 2) - headOriginX) * 2;
            }

            return new Vector2(headOriginX, avatarJson.HeadOrigin.Y);
        }

        public Rectangle GetSourceRectangle()
        {
            var animation = Animations[Animation];

            return new Rectangle()
            {
                X = (animation.X + AnimationFrame) * Size.X,
                Y = animation.Y * Size.Y + (IsSpinning ? 1 : 0),
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
            }

            if (!IsSpinning)
            {
                SpinningAngle = (float)(Math.PI / -2f);
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

            return CollisionHelper.OffsetRectangle(CollisionBox, Position);
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
        }
        public void SetY(float y)
        {
            Position = new Vector2(Position.X, y);
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetDirection(Direction direction)
        {
            if (Direction != direction)
            {
                Direction = direction;
            }
        }

        public void SetBufferedAnimiation(Animation bufferedAnimation)
        {
            BufferedAnimation = bufferedAnimation;
        }

        public Animation? PopBufferedAnimation()
        {
            if (BufferedAnimation != null)
            {
                var temp = BufferedAnimation;
                BufferedAnimation = null;
                return temp;
            }

            return null;
        }

        public void SetBufferedDirection(Direction bufferedDirection)
        {
            BufferedDirection = bufferedDirection;
        }

        public Direction? PeekBufferedDirection()
        {
            return BufferedDirection;
        }

        public Direction? PopBufferedDirection()
        {
            if (BufferedDirection != null)
            {
                var temp = BufferedDirection;
                BufferedDirection = null;
                return temp;
            }

            return null;
        }

        public bool HasBufferedAnimation()
        {
            return BufferedAnimation != null;
        }
    }
}
