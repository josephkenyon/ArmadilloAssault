using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Physics
{
    public static class PhysicsManager
    {
        private static readonly float gravityAcceleration = 0.035f;
         
        public static void UpdateAvatar(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            avatar.Grounded = false;

            ApplyHorizontalMotion(avatar, sceneCollisionBoxes);
            ApplyVerticalMotion(avatar, sceneCollisionBoxes);
        }

        private static void ApplyHorizontalMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var newVelocityX = avatar.Velocity.X + avatar.Acceleration.X + avatar.RunningVelocity + avatar.InfluenceVelocity;

            var ifMovingRight = newVelocityX > 0;
            if (ifMovingRight)
            {
                var avatarCollisionBox = avatar.GetCollisionBox();
                var avatarRightX = avatarCollisionBox.Right;
                var boxCandidates = sceneCollisionBoxes.Where(box => box.Left >= avatarRightX || box.Intersects(avatarCollisionBox));

                DrawingManager.DrawCollisionBoxes(sceneCollisionBoxes);

                var boxesThatIntersectInY = boxCandidates.Where(box => CollisionHelper.RectanglesIntersectInTheYPlane(avatarCollisionBox, box));

                var LeftMostBox = boxesThatIntersectInY.OrderBy(box => box.Left).FirstOrDefault(new Rectangle(1920 + Avatar.spriteWidth, 0, 1, 1));

                var wallX = LeftMostBox.Left;

                if (avatarRightX == wallX)
                {
                    avatar.Position = new Vector2((int) avatar.Position.X, avatar.Position.Y);
                    return;
                }

                if (avatarRightX + newVelocityX >= wallX)
                {
                    newVelocityX = 0;
                    avatar.Position = new Vector2((int)avatar.Position.X - (avatarRightX - wallX), avatar.Position.Y);
                }
                else
                {
                    avatar.Position = new Vector2(avatar.Position.X + newVelocityX, avatar.Position.Y);
                }
            }
            else if (newVelocityX < 0)
            {
                var avatarCollisionBox = avatar.GetCollisionBox();
                var avatarLeftX = avatarCollisionBox.Left;
                var boxCandidates = sceneCollisionBoxes.Where(box => box.Right <= avatarLeftX || box.Intersects(avatarCollisionBox));

                var boxesThatIntersectInY = boxCandidates.Where(box => CollisionHelper.RectanglesIntersectInTheYPlane(avatarCollisionBox, box));

                var leftMostBox = boxesThatIntersectInY.OrderByDescending(box => box.Right).FirstOrDefault(new Rectangle(-Avatar.spriteWidth, 0, 1, 1));

                var wallX = leftMostBox.Right;

                if (avatarLeftX == wallX)
                {
                    avatar.Position = new Vector2((int)avatar.Position.X, avatar.Position.Y);
                    return;
                }

                if (avatarLeftX + newVelocityX <= wallX)
                {
                    newVelocityX = 0;
                    avatar.Position = new Vector2(wallX - (avatarLeftX - (int)avatar.Position.X), avatar.Position.Y);
                }
                else
                {
                    avatar.Position = new Vector2(avatar.Position.X + newVelocityX, avatar.Position.Y);
                }
            }

            DecelerateX(avatar);
        }

        private static void DecelerateX(Avatar avatar)
        {
            if (PhysicsHelper.FloatsAreEqual(avatar.Acceleration.X, 0))
            {
                avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);

                var decelerationConstant = avatar.Grounded ? avatar.RunningAcceleration : 4f;

                if (avatar.Velocity.X > 0)
                {
                    var newVelocityX = Math.Max(0, avatar.Velocity.X - decelerationConstant);
                    avatar.Velocity = new Vector2(newVelocityX, avatar.Velocity.Y);
                }
                else if (avatar.Velocity.X < 0)
                {
                    var newVelocityX = Math.Min(0, avatar.Velocity.X + decelerationConstant);
                    avatar.Velocity = new Vector2(newVelocityX, avatar.Velocity.Y);
                }
            }
        }

        private static void ApplyVerticalMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            avatar.Velocity = new Vector2(avatar.Velocity.X, avatar.Velocity.Y + avatar.Acceleration.Y);

            var isJumping = avatar.Velocity.Y < 0;
            if (isJumping)
            {
                var avatarCollisionBox = avatar.GetCollisionBox();
                var avatarTopY = avatarCollisionBox.Top;
                var boxCandidates = sceneCollisionBoxes.Where(box => box.Bottom <= avatarTopY || box.Intersects(avatarCollisionBox));

                var boxesThatIntersectInX = boxCandidates.Where(box => CollisionHelper.RectanglesIntersectInTheXPlane(avatarCollisionBox, box));

                var lowestBox = boxesThatIntersectInX.OrderByDescending(box => box.Bottom).FirstOrDefault(new Rectangle(0, 0 - Avatar.spriteHeight, 1, 1));

                var ceilingY = lowestBox.Bottom;

                if (avatarTopY == ceilingY)
                {
                    avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                    avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                    avatar.Position = new Vector2(avatar.Position.X, (int)avatar.Position.Y);
                    return;
                }

                var yDelta = Math.Min(avatar.Velocity.Y, avatar.MaxVelocity.Y);

                if (avatarTopY + yDelta <= ceilingY)
                {
                    avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                    avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                    avatar.Position = new Vector2(avatar.Position.X, (int)avatar.Position.Y + (ceilingY - avatarTopY));
                }
                else
                {
                    avatar.Position = new Vector2(avatar.Position.X, avatar.Position.Y + yDelta);
                }
            }
            else
            {
                var avatarCollisionBox = avatar.GetCollisionBox();
                var avatarBottomY = avatarCollisionBox.Bottom;
                var boxCandidates = sceneCollisionBoxes.Where(box => box.Top >= avatarBottomY || box.Intersects(avatarCollisionBox));

                var boxesThatIntersectInX = boxCandidates.Where(box => CollisionHelper.RectanglesIntersectInTheXPlane(avatarCollisionBox, box));

                var highestBox = boxesThatIntersectInX.OrderBy(box => box.Top).FirstOrDefault(new Rectangle(0, 1080 + Avatar.spriteHeight, 1, 1));

                var floorY = highestBox.Top;

                if (avatarBottomY == floorY)
                {
                    avatar.Position = new Vector2(avatar.Position.X, (int)avatar.Position.Y);
                    avatar.Grounded = true;
                    avatar.AvailableJumps = 1;

                    avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                    avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                    return;
                }

                var yDelta = Math.Min(avatar.Velocity.Y, avatar.MaxVelocity.Y);

                if (avatarBottomY + yDelta >= floorY)
                {
                    avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                    avatar.Position = new Vector2(avatar.Position.X, (int)avatar.Position.Y + (floorY - avatarBottomY));
                }
                else
                {
                    avatar.Position = new Vector2(avatar.Position.X, avatar.Position.Y + yDelta);
                }
            }

            DecelerateY(avatar);
        }

        private static void DecelerateY(Avatar avatar)
        {
            if (PhysicsHelper.FloatsAreEqual(avatar.Acceleration.Y, 0) || avatar.Grounded)
            {
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
            }

            if (!avatar.Grounded)
            {
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, avatar.Acceleration.Y + gravityAcceleration);
            }
        }
    }
}
