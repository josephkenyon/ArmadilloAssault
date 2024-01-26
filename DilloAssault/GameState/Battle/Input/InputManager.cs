using DilloAssault.Controls;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Input
{
    public static class InputManager
    {
        internal static void UpdateAvatar(int playerIndex, Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var holdingLeft = ControlsManager.IsControlDown(playerIndex, Control.Left);
            var holdingRight = ControlsManager.IsControlDown(playerIndex, Control.Right);

            HandleMovement(playerIndex, avatar, sceneCollisionBoxes);
            UpdateAimDirection(playerIndex, avatar);

            var rolling = avatar.Animation == Animation.Rolling;

            var notTryingToMove = !holdingLeft && !holdingRight;

            if (notTryingToMove && avatar.Grounded && !rolling)
            {
                avatar.SetAnimation(Animation.Resting);
            }

            if (notTryingToMove || avatar.Grounded)
            {
                avatar.InfluenceVelocity = 0;
            }

            if (notTryingToMove || !avatar.Grounded)
            {
                if (avatar.RunningVelocity > 0)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - avatar.RunningAcceleration, 0, avatar.RunningVelocity);
                }
                else if (avatar.RunningVelocity < 0)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + avatar.RunningAcceleration, avatar.RunningVelocity, 0);
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
                        avatar.Direction = Direction.Left;

                        avatar.Position = new Vector2(avatar.Position.X + 10f, avatar.Position.Y);
                    }
                    else if (!holdingLeft && holdingRight)
                    {
                        avatar.Direction = Direction.Right;

                        avatar.Position = new Vector2(avatar.Position.X - 10f, avatar.Position.Y);
                    }
                }
                else if (avatar.Animation == Animation.Rolling)
                {
                    avatar.Position = new Vector2(avatar.Position.X, avatar.Position.Y - 48f);
                }

                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                avatar.Velocity = new Vector2(avatar.Velocity.X, -13.5f);
            }

            if (ControlsManager.IsControlDownStart(playerIndex, Control.Down) && avatar.Grounded)
            {
                avatar.SetAnimation(Animation.Rolling);
                PhysicsManager.MoveIfIntersecting(avatar, sceneCollisionBoxes);
            }
            else if (ControlsManager.IsControlDownStart(playerIndex, Control.Up) && avatar.Animation == Animation.Rolling)
            {
                if (avatar.Grounded)
                {
                    var ceiling = PhysicsManager.GetCeiling(avatar, sceneCollisionBoxes);

                    if (avatar.GetCollisionBox().Top - ceiling > 48)
                    {
                        avatar.SetAnimation(Animation.Resting);
                        avatar.Position = new Vector2(avatar.Position.X, avatar.Position.Y - 48);
                    }
                }
                else
                {
                    avatar.SetAnimation(Animation.Spinning);
                }
            }
        }

        private static void UpdateAimDirection(int playerIndex, Avatar avatar)
        {
            var origin = avatar.Position + avatar.GetArmOrigin();
            avatar.AimDirection = ControlsManager.GetAimPosition(playerIndex) - origin;

            if (avatar.Direction == Direction.Right)
            {
                if (avatar.AimDirection.X < 0)
                {
                    if (avatar.AimDirection.Y < 0)
                    {
                        avatar.AimAngle = Math.PI / -2;
                    }
                    else
                    {
                        avatar.AimAngle = 55 * Math.PI / 180;
                    }
                }

                if (avatar.AimDirection.X > 0 && avatar.AimDirection.Y > 0)
                {
                    avatar.AimAngle = Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));


                    if ((avatar.AimAngle * 180 / Math.PI) > 55)
                    {
                        avatar.AimAngle = 55 * Math.PI / 180;
                    }
                }
                else if (avatar.AimDirection.X > 0 && avatar.AimDirection.Y < 0)
                {
                    avatar.AimAngle = -1f * Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));
                }
            }
            else
            {
                if (avatar.AimDirection.X > 0)
                {
                    if (avatar.AimDirection.Y > 0)
                    {
                        avatar.AimAngle = (-55) * Math.PI / 180;
                    }
                    else
                    {
                        avatar.AimAngle = Math.PI / 2;
                    }
                }

                if (avatar.AimDirection.X < 0 && avatar.AimDirection.Y > 0)
                {
                    avatar.AimAngle = -Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));


                    if ((avatar.AimAngle * 180 / Math.PI) < -55)
                    {
                        avatar.AimAngle = -55 * Math.PI / 180;
                    }
                }
                else if (avatar.AimDirection.X < 0 && avatar.AimDirection.Y < 0)
                {
                    avatar.AimAngle = 1f * Math.Atan(Math.Abs(avatar.AimDirection.Y) / Math.Abs(avatar.AimDirection.X));
                }
            }
        }

        private static void HandleMovement(int playerIndex, Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            if (ControlsManager.IsControlDownStart(playerIndex, Control.Down))
            {
                avatar.SetAnimation(Animation.Rolling);
            }

            if (avatar.AimDirection.X > 0)
            {
                if (avatar.Direction == Direction.Left && !avatar.IsSpinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Direction = Direction.Right;

                    PhysicsManager.MoveIfIntersecting(avatar, sceneCollisionBoxes);
                }
            }
            else if (avatar.AimDirection.X < 0)
            {
                if (avatar.Direction == Direction.Right && !avatar.IsSpinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Direction = Direction.Left;

                    PhysicsManager.MoveIfIntersecting(avatar, sceneCollisionBoxes);
                }
            }

            if (ControlsManager.IsControlDown(playerIndex, Control.Left) || ControlsManager.IsControlDownStart(playerIndex, Control.Left))
            {
                avatar.RunningBackwards = avatar.Direction == Direction.Right && !avatar.IsSpinning;

                if (avatar.Grounded)
                {
                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.SetAnimation(Animation.Running);

                        avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                        avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - avatar.RunningAcceleration, -avatar.MaxRunningVelocity, avatar.MaxRunningVelocity);
                    }
                    else
                    {
                        avatar.Direction = Direction.Left;
                        avatar.IncrementSpin();

                        avatar.Acceleration = new Vector2(-avatar.RunningAcceleration, avatar.Acceleration.Y);
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
                        avatar.SetAnimation(Animation.Running);

                        avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);
                        avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + avatar.RunningAcceleration, -avatar.MaxRunningVelocity, avatar.MaxRunningVelocity);
                    }
                    else
                    {
                        avatar.Direction = Direction.Right;
                        avatar.IncrementSpin();

                        avatar.Acceleration = new Vector2(avatar.RunningAcceleration, avatar.Acceleration.Y);
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
        }
    }
}
