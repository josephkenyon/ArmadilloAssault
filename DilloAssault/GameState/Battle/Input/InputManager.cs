using DilloAssault.Controls;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.GameState.Battle.Input
{
    public static class InputManager
    {
        internal static void UpdateAvatar(int playerIndex, Avatar avatar)
        {
            var holdingLeft = ControlsManager.IsControlDown(playerIndex, Control.Left);
            var holdingRight = ControlsManager.IsControlDown(playerIndex, Control.Right);

            HandleMovement(playerIndex, avatar);
            UpdateAimDirection(playerIndex, avatar);

            var notTryingToMove = !holdingLeft && !holdingRight;

            if (notTryingToMove && avatar.Grounded)
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
                avatar.AvailableJumps--;
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                avatar.Velocity = new Vector2(avatar.Velocity.X, -13.5f);
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

        private static void HandleMovement(int playerIndex, Avatar avatar)
        {
            if (avatar.AimDirection.X > 0)
            {
                if (avatar.Direction == Direction.Left && avatar.Animation != Animation.Spinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Position = new Vector2(avatar.Position.X + 10, avatar.Position.Y);
                    avatar.Direction = Direction.Right;
                }
            }
            else if (avatar.AimDirection.X < 0)
            {
                if (avatar.Direction == Direction.Right && avatar.Animation != Animation.Spinning)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Position = new Vector2(avatar.Position.X - 8, avatar.Position.Y);
                    avatar.Direction = Direction.Left;
                }
            }

            if (ControlsManager.IsControlDown(playerIndex, Control.Left) || ControlsManager.IsControlDownStart(playerIndex, Control.Left))
            {
                avatar.RunningBackwards = avatar.Direction == Direction.Right;

                if (avatar.Grounded)
                {
                    avatar.SetAnimation(Animation.Running);
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - avatar.RunningAcceleration, -avatar.MaxVelocity.X, avatar.MaxVelocity.X);
                }
                else
                {
                    avatar.InfluenceVelocity = -4;
                }
            }
            else if (ControlsManager.IsControlDown(playerIndex, Control.Right) || ControlsManager.IsControlDownStart(playerIndex, Control.Right))
            {
                avatar.RunningBackwards = avatar.Direction == Direction.Left;

                if (avatar.Grounded)
                {
                    avatar.SetAnimation(Animation.Running);
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + avatar.RunningAcceleration, -avatar.MaxVelocity.X, avatar.MaxVelocity.X);
                }
                else
                {
                    avatar.InfluenceVelocity = 4;
                }
            }
        }
    }
}
