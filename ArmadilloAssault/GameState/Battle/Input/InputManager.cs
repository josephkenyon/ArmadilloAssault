using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Sound;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Input
{
    public static class InputManager
    {
        internal static void UpdateAvatar(int playerIndex, Avatar avatar)
        {
            if (avatar.HasBufferedAnimation())
            {
                return;
            }

            HandleMovement(playerIndex, avatar);
            UpdateAimDirection(playerIndex, avatar);
            HandleWeaponControls(playerIndex, avatar);
        }

        private static void HandleWeaponControls(int playerIndex, Avatar avatar)
        {
            var clickedFire = ControlsManager.IsControlDownStart(playerIndex, Control.Fire_Primary);
            var holdingFire = ControlsManager.IsControlDown(playerIndex, Control.Fire_Primary);

            if (ControlsManager.IsControlDownStart(playerIndex, Control.Cycle_Weapon))
            {
                avatar.CycleWeapon();
            }
            else if (ControlsManager.IsControlDownStart(playerIndex, Control.Reload))
            {
                avatar.Reload();
            }
            else if (clickedFire || (holdingFire && avatar.HoldingAutomaticWeapon) || avatar.BufferedShotFrameCounter > 0)
            {
                if (avatar.CanFire)
                {
                    avatar.Fire();
                }
                else if (avatar.SelectedWeapon.AmmoInClip == 0 && avatar.SelectedWeapon.CanReload())
                {
                    avatar.Reload();
                }
                else if (ControlsManager.IsControlDownStart(playerIndex, Control.Fire_Primary))
                {
                    avatar.BufferedShotFrameCounter = 24;
                }
                else
                {
                    avatar.BufferedShotFrameCounter--;
                }
            }
        }

        private static void UpdateAimDirection(int playerIndex, Avatar avatar)
        {
            var nullableAimPosition = ControlsManager.GetNullableAimPosition(playerIndex);

            if (nullableAimPosition == null)
            {
                return;
            }

            var aimPosition = (Vector2)nullableAimPosition;

            var origin = avatar.Position + avatar.GetArmOrigin();

            avatar.AimDirection = aimPosition - origin;

            var direction = avatar.PeekBufferedDirection() ?? avatar.Direction;

            avatar.AimAngle = (float)Math.Atan2(avatar.AimDirection.Y, avatar.AimDirection.X);

            if (direction == Direction.Right)
            {
                if (avatar.AimDirection.X < 0)
                {
                    if (avatar.AimDirection.Y < 0)
                    {
                        avatar.ArmAngle = Math.PI / -2;
                    }
                    else
                    {
                        avatar.ArmAngle = 55 * Math.PI / 180;
                    }
                }

                if (avatar.AimDirection.X > 0 && avatar.AimDirection.Y > 0)
                {
                    avatar.ArmAngle = Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));


                    if ((avatar.ArmAngle * 180 / Math.PI) > 55)
                    {
                        avatar.ArmAngle = 55 * Math.PI / 180;
                    }
                }
                else if (avatar.AimDirection.X > 0 && avatar.AimDirection.Y < 0)
                {
                    avatar.ArmAngle = -1f * Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));
                }
            }
            else
            {
                if (avatar.AimDirection.X > 0)
                {
                    if (avatar.AimDirection.Y > 0)
                    {
                        avatar.ArmAngle = (-55) * Math.PI / 180;
                    }
                    else
                    {
                        avatar.ArmAngle = Math.PI / 2;
                    }
                }

                if (avatar.AimDirection.X < 0 && avatar.AimDirection.Y > 0)
                {
                    avatar.ArmAngle = -Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));


                    if ((avatar.ArmAngle * 180 / Math.PI) < -55)
                    {
                        avatar.ArmAngle = -55 * Math.PI / 180;
                    }
                }
                else if (avatar.AimDirection.X < 0 && avatar.AimDirection.Y < 0)
                {
                    avatar.ArmAngle = 1f * Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));
                }
            }

            if (avatar.AimDirection.X > 0)
            {
                if (avatar.Direction == Direction.Left && !avatar.IsSpinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.SetBufferedDirection(Direction.Right);
                }
            }
            else if (avatar.AimDirection.X < 0)
            {
                if (avatar.Direction == Direction.Right && !avatar.IsSpinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.SetBufferedDirection(Direction.Left);
                }
            }
        }

        private static void HandleMovement(int playerIndex, Avatar avatar)
        {
            var holdingLeft = ControlsManager.IsControlDown(playerIndex, Control.Left);
            var holdingRight = ControlsManager.IsControlDown(playerIndex, Control.Right);

            if (ControlsManager.IsControlDown(playerIndex, Control.Left) || ControlsManager.IsControlDownStart(playerIndex, Control.Left))
            {
                avatar.RunningBackwards = avatar.Direction == Direction.Right && !avatar.IsSpinning;

                if (avatar.Grounded)
                {
                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.BufferAnimation(Animation.Running);

                        avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                        avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - AvatarConstants.RunningAcceleration, -AvatarConstants.MaxRunningVelocity, AvatarConstants.MaxRunningVelocity);
                    }
                    else
                    {
                        avatar.SetDirection(Direction.Left);
                        avatar.IncrementSpin();

                        avatar.Acceleration = new Vector2(-AvatarConstants.RunningAcceleration, avatar.Acceleration.Y);
                    }
                }
                else
                {
                    avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                    avatar.InfluenceVelocity = -4;
                }
            }
            else if (ControlsManager.IsControlDown(playerIndex, Control.Right) || ControlsManager.IsControlDownStart(playerIndex, Control.Right))
            {
                avatar.RunningBackwards = avatar.Direction == Direction.Left && !avatar.IsSpinning;

                if (avatar.Grounded)
                {
                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.BufferAnimation(Animation.Running);

                        avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                        avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + AvatarConstants.RunningAcceleration, -AvatarConstants.MaxRunningVelocity, AvatarConstants.MaxRunningVelocity);
                    }
                    else
                    {
                        avatar.SetDirection(Direction.Right);
                        avatar.IncrementSpin();

                        avatar.Acceleration = new Vector2(AvatarConstants.RunningAcceleration, avatar.Acceleration.Y);
                    }
                }
                else
                {
                    avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                    avatar.InfluenceVelocity = 4;
                }
            }
            else
            {
                avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
            }


            var notTryingToMove = !holdingLeft && !holdingRight;

            if (notTryingToMove && avatar.Grounded && !avatar.IsSpinning)
            {
                avatar.BufferAnimation(Animation.Resting);
            }

            if (notTryingToMove || avatar.Grounded)
            {
                avatar.InfluenceVelocity = 0;
            }

            if (notTryingToMove || !avatar.Grounded)
            {
                if (avatar.RunningVelocity > 0)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - AvatarConstants.RunningAcceleration, 0, avatar.RunningVelocity);
                }
                else if (avatar.RunningVelocity < 0)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + AvatarConstants.RunningAcceleration, avatar.RunningVelocity, 0);
                }
            }

            if (ControlsManager.IsControlDownStart(playerIndex, Control.Jump) && avatar.AvailableJumps > 0)
            {
                if (avatar.IsSpinning)
                {
                    avatar.AvailableJumps = 0;
                }
                else
                {
                    avatar.AvailableJumps--;
                }

                if (avatar.AvailableJumps == 0)
                {
                    if (holdingLeft && !holdingRight)
                    {
                        avatar.SetBufferedDirection(Direction.Left);
                    }
                    else if (!holdingLeft && holdingRight)
                    {
                        avatar.SetBufferedDirection(Direction.Right);
                    }

                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.SetAnimation(Animation.Spinning);
                    }
                }

                avatar.Jumped = true;
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                avatar.Velocity = new Vector2(avatar.Velocity.X, -18f);
                SoundManager.QueueAvatarSound(avatar.Type, AvatarSound.Jump);
            }


            if (ControlsManager.IsControlDown(playerIndex, Control.Down) && avatar.Grounded)
            {
                avatar.DropThrough = true;
            }

            if (ControlsManager.IsControlDownStart(playerIndex, Control.Down))
            {
                if (avatar.IsSpinning)
                {
                    if (!avatar.Grounded)
                    {
                        avatar.BufferAnimation(Animation.Rolling);
                    }
                }
                else
                {
                    avatar.BufferAnimation(Animation.Rolling);
                }
            }
            else if (ControlsManager.IsControlDownStart(playerIndex, Control.Up) && avatar.IsSpinning)
            {
                if (avatar.Grounded)
                {
                    avatar.BufferAnimation(Animation.Resting);
                }
                else
                {
                    avatar.BufferAnimation(Animation.Spinning);
                }
            }
        }
    }
}
