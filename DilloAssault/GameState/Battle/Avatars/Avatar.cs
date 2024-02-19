﻿using DilloAssault.Configuration;
using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Textures;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.GameState.Battle.Weapons;
using DilloAssault.Generics;
using DilloAssault.Sound;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Avatars
{
    public class Avatar(AvatarJson avatarJson) : PhysicsObject
    {
        // Collision
        private readonly Rectangle CollisionBox = ConfigurationHelper.GetRectangle(avatarJson.CollisionBox);
        private readonly Rectangle SpinningCollisionBox = ConfigurationHelper.GetRectangle(avatarJson.SpinningCollisionBox);

        // Hurt Boxes
        private readonly IEnumerable<Rectangle> HurtBoxes = ConfigurationHelper.GetHurtBoxes(avatarJson.HurtBoxes);
        private readonly Rectangle ShellBox = ConfigurationHelper.GetRectangle(avatarJson.ShellBox);
        private readonly Rectangle SpinningHurtBox = ConfigurationHelper.GetRectangle(avatarJson.SpinningHurtBox);
        private readonly Rectangle SpinningShellBox = ConfigurationHelper.GetRectangle(avatarJson.SpinningShellBox);

        // Drawing
        public readonly TextureName TextureName = avatarJson.TextureName;
        public readonly Point Size = new(avatarJson.Size.X, avatarJson.Size.Y);
        public readonly Point SpriteOffset = new(avatarJson.SpriteOffset.X, avatarJson.SpriteOffset.Y);
        public Vector2 SpriteOffsetVector => new(Direction == Direction.Left ? -SpriteOffset.X : SpriteOffset.X, SpriteOffset.Y);

        // Animation
        private readonly Dictionary<Animation, AnimationJson> Animations = ConfigurationHelper.GetAnimations(avatarJson.Animations);
        public AvatarType Type { get; private set; } = avatarJson.Type;
        public Animation Animation { get; private set; } = Animation.Resting;
        private int FrameCounter { get; set; }
        public int AnimationFrame { get; private set; }

        public bool IsDead => Health < 1 || Animation == Animation.Dead;

        // Health
        private int health = 100;
        public int Health { get { return health; } set { health = Math.Clamp(value, 0, 100); } }

        // Physics
        public int AvailableJumps { get; set; }
        public bool RunningBackwards { get; set; }
        public bool IsSpinning => Animation == Animation.Spinning || Animation == Animation.Rolling;

        // Rotation
        public double ArmAngle { get; set; }
        public float AimAngle { get; set; }
        public float SpinningAngle { get; private set; } = (float)(Math.PI / -2f);
        public float Rotation => Direction == Direction.Left ? -SpinningAngle : SpinningAngle;
        public Vector2 AimDirection { get; set; }
        public Vector2 Origin => IsSpinning ? new Vector2(Size.X / 2, Size.Y / 2) : Vector2.Zero;
        public Vector2 OffsetOrigin => Origin + Position + SpriteOffsetVector;

        // Breathing
        public int BreathingFrameCounter { get; private set; }
        private bool BreathingIn { get; set; }

        // Buffers
        private Animation? BufferedAnimation { get; set; }
        private Direction? BufferedDirection { get; set; }

        // Weapons
        public List<Weapon> Weapons { get; private set; } = [new Weapon(ConfigurationManager.GetWeaponConfiguration(WeaponType.Pistol))];
        private int WeaponSelectionIndex { get; set; }
        public Weapon SelectedWeapon => Weapons[WeaponSelectionIndex];
        public WeaponJson CurrentWeaponConfiguration => ConfigurationManager.GetWeaponConfiguration(Weapons[WeaponSelectionIndex].Type);
        public bool HoldingAutomaticWeapon => SelectedWeapon.Type == WeaponType.Assault;
        public bool CanFire => !IsSpinning && Weapons.Count != 0 && Weapons[WeaponSelectionIndex].CanFire() && !Reloading && !SwitchingWeapons;
        public int BufferedShotFrameCounter { get; set; } = 0;

        public float GetRecoil => (SelectedWeapon.AmmoInClip == 0 && SelectedWeapon.Ammo == 0) ? (float)(Math.PI / 4) : Recoil;
        private float Recoil { get; set; }
        public bool HasRecoil { get; set; } = false;
        public int FramesUntilRecoil { get; set; } = -1;

        public bool Reloading { get; set; } = false;
        public int ReloadingFrames { get; set; } = 0;

        public bool SwitchingWeapons { get; set; } = false;
        public int SwitchingWeaponFrames { get; set; } = 0;
        public int FramesSinceLastHurtSound { get; set; } = 15;

        private static readonly int WeaponSwitchFrames = 10;

        public override bool LowDrag => IsSpinning;

        public bool Jumped { get; set; }

        public void Update()
        {
            UpdateFrameCounter();
            UpdateBreathingFrameCounter();
            UpdateRecoil();
            UpdateReloading();
            UpdateSwitchingWeapons();
            UpdatePhysics();
            UpdateSound();

            Weapons.ForEach(weapon => weapon.Update());
        }

        private void UpdateSound()
        {
            FramesSinceLastHurtSound++;
            if (Jumped)
            {
                SoundManager.PlayAvatarSound(avatarJson.Type, AvatarSound.Jump);
                Jumped = false;
            }
        }

        private void UpdatePhysics()
        {
            if (Grounded)
            {
                Falling = false;
                Rising = false;
                AvailableJumps = 2;
                if (Animation == Animation.Falling)
                {
                    SetAnimation(Animation.Resting);
                }
                else if (Animation == Animation.Spinning)
                {
                    BufferAnimation(Animation.Resting);
                }
            }
            else if (Falling)
            {
                Rising = false;

                if (AvailableJumps == 2)
                {
                    AvailableJumps = 1;
                }

                if (!IsSpinning)
                {
                    SetAnimation(Animation.Falling);
                }
                else
                {
                    IncrementSpin();
                }
            }
            else if (Rising)
            {
                if (AvailableJumps == 1)
                {
                    if (Animation != Animation.Rolling)
                    {
                        SetAnimation(Animation.Jumping);
                    }
                }
                else
                {
                    IncrementSpin();
                    if (Animation != Animation.Rolling)
                    {
                        SetAnimation(Animation.Spinning);
                    }
                }
            }
        }

        private void UpdateSwitchingWeapons()
        {
            if (SwitchingWeapons)
            {
                if (SwitchingWeaponFrames > 0)
                {
                    SwitchingWeaponFrames--;
                }
                else
                {
                    SwitchingWeapons = false;
                }
            }
        }

        private void UpdateReloading()
        {
            if (Reloading)
            {
                if (ReloadingFrames > 0)
                {
                    ReloadingFrames--;
                }
                else
                {
                    Reloading = false;
                    SelectedWeapon.Reload();
                }
            }
        }

        private void UpdateRecoil()
        {
            if (FramesUntilRecoil >= 0)
            {
                if (FramesUntilRecoil == 0) {
                    Recoil = (float)(CurrentWeaponConfiguration.RecoilStrength * 45 * Math.PI / 180);
                }

                FramesUntilRecoil--;
            }

            if (Recoil > 0)
            {
                if (SwitchingWeapons)
                {
                    Recoil -= (float)(Math.PI / 2 / WeaponSwitchFrames);
                }
                else if (Reloading)
                {
                    Recoil -= (float)(Math.PI / 2 / CurrentWeaponConfiguration.ReloadRate);
                }
                else if (HasRecoil)
                {
                    Recoil -= (float)(Math.PI / 4 / CurrentWeaponConfiguration.RecoilRecoveryRate);
                }
            }

            if (Recoil < 0)
            {
                Recoil = 0;
                HasRecoil = false;
            }
        }

        public void Fire()
        {
            var weaponTip = GetWeaponTip();

            FramesUntilRecoil = 6;
            HasRecoil = true;

            var currentWeapon = Weapons[WeaponSelectionIndex];
            currentWeapon.Fire(weaponTip, AimAngle, Direction);

            if (currentWeapon.AmmoInClip == 0 && currentWeapon.CanReload())
            {
                Reload();
            }

            SoundManager.PlayWeaponSound(currentWeapon.Type);

            BufferedShotFrameCounter = 0;
        }

        public void Reload()
        {
            if (!Reloading && !SelectedWeapon.HasFullClip() && SelectedWeapon.CanReload())
            {
                ReloadingFrames = CurrentWeaponConfiguration.ReloadRate;
                Reloading = true;
                Recoil = (float)(Math.PI / 2);
                FramesUntilRecoil = -1;
            }

            BufferedShotFrameCounter = 0;
        }

        public void CycleWeapon()
        {
            if (Weapons.Count > WeaponSelectionIndex + 1)
            {
                WeaponSelectionIndex++;
                if (SelectedWeapon.Ammo != 0 || SelectedWeapon.AmmoInClip != 0)
                {
                    HandleWeaponChange();
                }
                else
                {
                    CycleWeapon();
                }
            }
            else if (WeaponSelectionIndex != 0)
            {
                WeaponSelectionIndex = 0;
                HandleWeaponChange();
            }
        }

        private void HandleWeaponChange()
        {
            SwitchingWeaponFrames = WeaponSwitchFrames;
            Recoil = (float)(Math.PI / 2);
            FramesUntilRecoil = -1;
            ReloadingFrames = 0;
            SwitchingWeapons = true;    

            var newWeapon = Weapons[WeaponSelectionIndex];
            if (newWeapon.AmmoInClip == 0)
            {
                Reload();
            }
        }

        private void UpdateFrameCounter()
        {
            var animation = Animations[Animation];

            if (animation.FrameCount == 1)
            {
                FrameCounter = 0;
                AnimationFrame = 0;

            }
            else
            {
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
        }

        private void UpdateBreathingFrameCounter()
        {
            if (BreathingFrameCounter == AvatarConstants.BreathingCycleFrameLength * 2)
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

        public Vector2 GetCenter()
        {
            return new Vector2(Position.X + (Size.X / 2), Position.Y + (Size.Y / 2));
        }

        public void HitByBullet(Bullet bullet, bool headShot)
        {
            var wasAlive = Health > 1;
            var damage = ConfigurationManager.GetWeaponConfiguration(bullet.WeaponType).BulletDamage;
            Health -= headShot ? (int)(damage * 1.5f) : damage;
            
            if (IsDead)
            {
                if (FramesSinceLastHurtSound > 15 || wasAlive)
                {
                    FramesSinceLastHurtSound = 0;
                    SoundManager.PlayAvatarSound(avatarJson.Type, AvatarSound.Dead);
                }
                Animation = Animation.Dead;
            }
            else if (FramesSinceLastHurtSound > 15)
            {
                FramesSinceLastHurtSound = 0;
                SoundManager.PlayAvatarSound(avatarJson.Type, AvatarSound.Hurt);
            }
        }
        
        private Vector2 GetWeaponTip()
        {
            var weaponOffset = CurrentWeaponConfiguration.SpriteOffset;

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
            var B = AvatarConstants.BreathingCycleFrameLength * 2;
            var X = Math.Clamp(BreathingFrameCounter, 0, AvatarConstants.BreathingCycleFrameLength);
            return (float)(A * Math.Sin(X * Math.PI / B)) - 3;
        }

        public Vector2 GetArmOrigin()
        {
            var armOriginX = avatarJson.ArmOrigin.X;
            var armOriginY = avatarJson.ArmOrigin.Y;

            armOriginX += SpriteOffset.X / 2;

            return new Vector2(armOriginX, armOriginY);
        }

        public void SetAnimation(Animation animation)
        {
            if (Animation != Animation.Dead)
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
        }

        public void IncrementSpin()
        {
            var velocityX = Math.Clamp(Velocity.X, -MaxVelocity.X * 2f, MaxVelocity.X * 2f) * 0.007f;
            var velocityY = Math.Clamp(Velocity.Y, -MaxVelocity.Y * 2f, MaxVelocity.Y * 2f) * 0.007f;

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

        public override Rectangle GetCollisionBox()
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
                if (IsSpinning)
                {
                    return [CollisionHelper.OffsetRectangle(SpinningHurtBox, Position + SpriteOffsetVector)];
                }

                return HurtBoxes.Select(box => CollisionHelper.OffsetRectangle(box, Position + SpriteOffsetVector));
            }
            else
            {
                if (IsSpinning)
                {
                    return [CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(SpinningHurtBox, AvatarConstants.spriteWidth), Position + SpriteOffsetVector)];
                }

                return HurtBoxes.Select(box => CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(box, AvatarConstants.spriteWidth), Position + SpriteOffsetVector));
            }
        }

        public Rectangle GetShellBox()
        {
            var box = IsSpinning == true ? SpinningShellBox : ShellBox;

            if (Direction == Direction.Right)
            {
                return CollisionHelper.OffsetRectangle(box, Position + SpriteOffsetVector);
            }
            else
            {
                return CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(box, AvatarConstants.spriteWidth), Position + SpriteOffsetVector);
            }
        }

        public void SetDirection(Direction direction)
        {
            if (Direction != direction)
            {
                Direction = direction;

                if (IsSpinning)
                {
                    Position -= SpriteOffsetVector;
                }
            }
        }

        public void BufferAnimation(Animation bufferedAnimation)
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

        public void GiveWeapon(WeaponType weaponType)
        {
            var configuration = ConfigurationManager.GetWeaponConfiguration(weaponType);
            var weapon = Weapons.SingleOrDefault(weapon => weapon.Type == weaponType);
            if (weapon != null)
            {
                weapon.Ammo += configuration.ClipsGiven * configuration.ClipSize;
                WeaponSelectionIndex = Weapons.IndexOf(weapon);
            }
            else
            {
                var newWeapon = new Weapon(configuration);
                Weapons.Add(newWeapon);

                WeaponSelectionIndex = Weapons.IndexOf(newWeapon);
            }
        }
    }
}
