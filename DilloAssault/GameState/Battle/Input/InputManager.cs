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

            if (holdingLeft || ControlsManager.IsControlDownStart(playerIndex, Control.Left))
            {
                if (avatar.Direction == Direction.Right)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Position = new Vector2(avatar.Position.X - 15, avatar.Position.Y);
                    avatar.Direction = Direction.Left;
                }

                if (avatar.Grounded)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity - avatar.RunningAcceleration, -avatar.MaxVelocity.X, avatar.MaxVelocity.X);
                }
                else
                {
                    avatar.InfluenceVelocity = -4;
                }
            }
            else if (holdingRight || ControlsManager.IsControlDownStart(playerIndex, Control.Right))
            {
                if (avatar.Direction == Direction.Left)
                {
                    avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                    avatar.Position = new Vector2(avatar.Position.X + 15, avatar.Position.Y);
                    avatar.Direction = Direction.Right;
                }

                if (avatar.Grounded)
                {
                    avatar.RunningVelocity = Math.Clamp(avatar.RunningVelocity + avatar.RunningAcceleration, -avatar.MaxVelocity.X, avatar.MaxVelocity.X);
                }
                else
                {
                    avatar.InfluenceVelocity = 4;
                }
            }

            var notTryingToMove = !holdingLeft && !holdingRight;

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
    }
}
