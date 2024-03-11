using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.GameState.Battle.PowerUps;
using ArmadilloAssault.GameState.Battle.Weapons;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Sound;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.Assets
{
    public class Avatar(int playerIndex, AvatarJson avatarJson, IAvatarListener avatarListener = null) : PhysicsObject
    {
        // Constants
        public static readonly int spriteWidth = 128;
        public static readonly int spriteHeight = 128;

        public static readonly float JumpingAcceleration = 0.5f;

        public static readonly int BreathingCycleFrameLength = 80;

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
        public int RespawnTimerFrames { get; internal set; } = -1;

        // Physics
        public int AvailableJumps { get; set; } = 1;

        private bool runningBackwards = false;
        public bool Scoped { get; set; }
        public bool RunningBackwards { get { return runningBackwards; } set { RunningFrameCount = runningBackwards != value && RunningFrameCount / 5 > 2 ? 0 : RunningFrameCount; runningBackwards = value; } }
        public bool IsSpinning => Animation == Animation.Spinning || Animation == Animation.Rolling;
        public int RunningFrameCount { get; set; }
        public int RollingFrameCount { get; set; }
        public override Vector2 MaxVelocity => SuperSpeed ? new(15f, 16f) : new(9f, 12f);

        public override float RunningAcceleration => SuperSpeed ? 1.2f : 0.65f;
        public float MaxRunningVelocity => SuperSpeed ? 13f : 6.5f;

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
        public List<Weapon> Weapons { get; private set; } = [new Weapon(ConfigurationManager.GetWeaponConfiguration(WeaponType.Pistol), avatarListener)];
        private int WeaponSelectionIndex { get; set; }
        public Weapon SelectedWeapon => Weapons[WeaponSelectionIndex];
        public WeaponJson CurrentWeaponConfiguration => ConfigurationManager.GetWeaponConfiguration(Weapons[WeaponSelectionIndex].Type);
        public bool HoldingAutomaticWeapon => SelectedWeapon.Type == WeaponType.Assault;
        public bool CanFire => !IsSpinning && Weapons.Count != 0 && Weapons[WeaponSelectionIndex].CanFire() && !Reloading && !SwitchingWeapons;
        public int BufferedShotFrameCounter { get; set; } = 0;

        public float GetRecoil => SelectedWeapon.AmmoInClip == 0 && SelectedWeapon.Ammo == 0 ? (float)(Math.PI / 4) : Recoil;
        private float Recoil { get; set; }
        public bool HasRecoil { get; set; } = false;
        public int FramesUntilRecoil { get; set; } = -1;

        public bool Reloading { get; set; } = false;
        public int ReloadingFrames { get; set; } = 0;

        // Power Ups
        public PowerUpType? CurrentPowerUp { get; set; } = null;
        public bool SuperSpeed => PowerUpType.Super_Speed == CurrentPowerUp;
        public int PowerUpFramesLeft { get; set; } = 0;

        public bool SwitchingWeapons { get; set; } = false;
        public int SwitchingWeaponFrames { get; set; } = 0;

        private static readonly int WeaponSwitchFrames = 10;

        public override float DragModifier => 1f * ((!Grounded && IsSpinning) ? 0.1f : 1f) * (SuperSpeed ? 1f : 1f);

        public bool Jumped { get; set; }
        public bool DropThrough { get; set; }

        public void Update()
        {
            UpdateRespawnTimer();
            UpdateFrameCounter();
            UpdateBreathingFrameCounter();
            UpdateRecoil();
            UpdateReloading();
            UpdateSwitchingWeapons();
            UpdatePowerUp();
            UpdatePhysics();

            Weapons.ForEach(weapon => weapon.Update());
        }

        private void UpdateRespawnTimer()
        {
            if (RespawnTimerFrames >= 0)
            {
                if (RespawnTimerFrames == 0)
                {
                    Respawn();
                }

                RespawnTimerFrames--;
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
                    SetAnimation(Animation.Resting);
                }

                if (Animation == Animation.Rolling && RollingFrameCount == (SuperSpeed ? 20 : 35))
                {
                    SoundManager.QueueBattleSound(BattleSound.rolling_grass);
                    RollingFrameCount = 0;
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

            if (!Grounded || Animation != Animation.Rolling)
            {
                RollingFrameCount = 0;
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

                    if (ReloadingFrames == 35)
                    {
                        SoundManager.QueueBattleSound(BattleSound.reload_end);
                    }
                }
                else
                {
                    Reloading = false;
                    SelectedWeapon.Reload();
                }
            }
        }

        private void UpdatePowerUp()
        {
            if (CurrentPowerUp != null)
            {
                PowerUpFramesLeft--;
                if (PowerUpFramesLeft == 0)
                {
                    CurrentPowerUp = null;
                }
            }
        }

        private void UpdateRecoil()
        {
            if (FramesUntilRecoil >= 0)
            {
                if (FramesUntilRecoil == 0)
                {
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
            currentWeapon.Fire(weaponTip, AimAngle, Direction, PowerUpType.Damage_Up == CurrentPowerUp ? 1.5f : 1.0f, playerIndex);

            if (currentWeapon.AmmoInClip == 0 && currentWeapon.CanReload())
            {
                Reload();
            }

            SoundManager.QueueWeaponSound(currentWeapon.Type);

            BufferedShotFrameCounter = 0;
        }

        public void Reload()
        {
            if (!Reloading && !SelectedWeapon.HasFullClip() && SelectedWeapon.CanReload())
            {
                ReloadingFrames = CurrentWeaponConfiguration.ReloadRate;
                Reloading = true;
                Recoil = (float)(Math.PI / 2);
                SwitchingWeapons = false;
                FramesUntilRecoil = -1;
                SoundManager.QueueBattleSound(BattleSound.reload);
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
            Reloading = false;
            SoundManager.CancelReloadSoundEffects();
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
                if (FrameCounter >= (SuperSpeed ? 2 : 3))
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

                    if (Animation == Animation.Running)
                    {
                        RunningFrameCount++;
                        if (RunningFrameCount == 5)
                        {
                            SoundManager.QueueBattleSound(BattleSound.footstep_grass);
                            RunningFrameCount = 0;
                        }
                    }
                    else
                    {
                        RunningFrameCount = 0;
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

        public Vector2 GetCenter()
        {
            return new Vector2(Position.X + Size.X / 2, Position.Y + Size.Y / 2);
        }

        public void HitByBullet(Bullet bullet, bool headShot)
        {
            var wasAlive = Health > 1;
            var damage = ConfigurationManager.GetWeaponConfiguration(bullet.WeaponType).BulletDamage * bullet.DamageModifier;

            if (headShot)
            {
                damage *= 1.5f;
            }

            avatarListener?.AvatarHit(playerIndex, bullet.PlayerIndex, (int)damage);

            Health -= (int)damage;

            if (IsDead)
            {
                if (wasAlive)
                {
                    SoundManager.QueueAvatarSound(avatarJson.Type, AvatarSound.Dead);
                }

                Animation = Animation.Dead;
                Acceleration = Vector2.Zero;
                Recoil = 0f;
                InfluenceVelocity = 0;
                RunningVelocity = 0;

                avatarListener?.AvatarKilled(playerIndex, bullet.PlayerIndex);
            }
            else
            {
                SoundManager.QueueAvatarSound(avatarJson.Type, AvatarSound.Hurt);
            }
        }

        private void Respawn()
        {
            Health = 100;
            Animation = Animation.Resting;
            Position = StartingPosition;
            PowerUpFramesLeft = 3 * 60;
            CurrentPowerUp = PowerUpType.Invincibility;

            WeaponSelectionIndex = 0;
            FramesUntilRecoil = -1;
            Reloading = false;
            SoundManager.CancelReloadSoundEffects();
            ReloadingFrames = 0;
            Recoil = 0;
            Weapons.Clear();
            Weapons.Add(new Weapon(ConfigurationManager.GetWeaponConfiguration(WeaponType.Pistol), avatarListener));
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

            if (Grounded && (!MathUtils.FloatsAreEqual(0f, Velocity.X) || !MathUtils.FloatsAreEqual(0f, Velocity.Y)))
            {
                RollingFrameCount++;
            }
        }

        public override Rectangle GetCollisionBox()
        {
            if (Animation == Animation.Spinning || Animation == Animation.Rolling)
            {
                return CollisionHelper.OffsetRectangle(SpinningCollisionBox, Position);
            }

            return GetFullCollisionBox();
        }

        public Rectangle GetFullCollisionBox()
        {
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
                    return [CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(SpinningHurtBox, spriteWidth), Position + SpriteOffsetVector)];
                }

                return HurtBoxes.Select(box => CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(box, spriteWidth), Position + SpriteOffsetVector));
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
                return CollisionHelper.OffsetRectangle(CollisionHelper.FlipRectangle(box, spriteWidth), Position + SpriteOffsetVector);
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
            if (Animation != bufferedAnimation)
            {
                BufferedAnimation = bufferedAnimation;
            }
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

            var weaponChange = weaponType != CurrentWeaponConfiguration.Type;

            if (weapon != null)
            {
                weapon.Ammo += configuration.ClipsGiven * configuration.ClipSize;
                WeaponSelectionIndex = Weapons.IndexOf(weapon);
            }
            else
            {
                var newWeapon = new Weapon(configuration, avatarListener);
                Weapons.Add(newWeapon);

                WeaponSelectionIndex = Weapons.IndexOf(newWeapon);
            }

            if (weaponChange)
            {
                HandleWeaponChange();
            }
        }

        public void GivePowerUp(PowerUpType powerUpType)
        {
            CurrentPowerUp = powerUpType;

            var seconds = powerUpType switch
            {
                PowerUpType.Invincibility => 8,
                PowerUpType.Damage_Up => 10,
                PowerUpType.Super_Speed => 12,
                PowerUpType.Invisibility => 10,
                _ => 10
            };

            PowerUpFramesLeft = seconds * 60;
        }

        public ColorJson GetColor()
        {
            var color = ColorJson.White;

            if (CurrentPowerUp != null)
            {
                var blinkFrame = PowerUpFramesLeft - ((PowerUpFramesLeft / 60) * 60);

                blinkFrame = (int)(255f - (blinkFrame * 255f / 60f));

                switch ((PowerUpType)CurrentPowerUp)
                {
                    case PowerUpType.Damage_Up:
                        return new ColorJson(255, blinkFrame, blinkFrame);
                    case PowerUpType.Super_Speed:
                        return new ColorJson(blinkFrame, 255, blinkFrame);
                    case PowerUpType.Invincibility:
                        return new ColorJson(blinkFrame, blinkFrame, 255);
                    default:
                        return new ColorJson(255, 255, 255);
                }
            }

            return color;
        }

        public string GetRespawnMessage()
        {
            if (RespawnTimerFrames <= 0)
            {
                return "";
            }

            var seconds = RespawnTimerFrames / 60;

            return "" + (seconds + 1);
        }
    }
}
