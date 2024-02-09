using DilloAssault.Configuration.Avatars;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.Generics;
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
            avatar.CloseToGround = false;

            var bufferedDirection = avatar.PopBufferedDirection();
            if (bufferedDirection != null)
            {
                avatar.SetDirection((Direction)bufferedDirection);
            }

            var bufferedAnimation = avatar.PopBufferedAnimation();
            if (bufferedAnimation != null)
            {
                if (bufferedAnimation == Animation.Resting && avatar.IsSpinning)
                {
                    if (CanExitSpinning(avatar, sceneCollisionBoxes))
                    {
                        avatar.SetAnimation((Animation)bufferedAnimation);
                        avatar.SetY(avatar.Position.Y - 44);
                    }
                    else
                    {
                        avatar.SetAnimation(Animation.Rolling);
                    }
                }
                else
                {
                    avatar.SetAnimation((Animation)bufferedAnimation);
                }
            }

            ApplyHorizontalMotion(avatar, sceneCollisionBoxes);
            ApplyVerticalMotion(avatar, sceneCollisionBoxes);
        }

        private static bool CanExitSpinning(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var collisionBox = avatar.GetCollisionBox();
            var ceiling = GetCeiling(collisionBox, sceneCollisionBoxes);

            return collisionBox.Top - ceiling > 64;
        }

        private static void ApplyHorizontalMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            avatar.Velocity = new Vector2(
                Math.Clamp(
                    avatar.Velocity.X + avatar.Acceleration.X,
                    -AvatarConstants.MaxVelocity.X, AvatarConstants.MaxVelocity.X
                ),
                avatar.Velocity.Y
            );

            var xVelocity = avatar.Velocity.X + avatar.RunningVelocity + avatar.InfluenceVelocity;

            if (xVelocity > 0)
            {
                ApplyRightMotion(avatar, sceneCollisionBoxes, xVelocity);
            }
            else if (xVelocity < 0)
            {
                ApplyLeftMotion(avatar, sceneCollisionBoxes, xVelocity);
            }

            DecelerateX(avatar);
        }

        private static void ApplyVerticalMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            avatar.Velocity = new Vector2(
                avatar.Velocity.X,
                Math.Clamp(
                    avatar.Velocity.Y + avatar.Acceleration.Y,
                    -AvatarConstants.MaxVelocity.Y, AvatarConstants.MaxVelocity.Y
                )
            );

            if (avatar.Velocity.Y < 0)
            {
                ApplyUpwardMotion(avatar, sceneCollisionBoxes);
            }
            else
            {
                ApplyDownwardMotion(avatar, sceneCollisionBoxes);
            }

            DecelerateY(avatar);
        }

        private static void ApplyLeftMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes, float deltaX)
        {
            var avatarCollisionBox = avatar.GetCollisionBox();
            var wallX = GetLeftWall(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Left == wallX)
            {
                avatar.SetX((int)avatar.Position.X);
                avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                return;
            }

            if (avatarCollisionBox.Left + deltaX <= wallX)
            {
                avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                avatar.SetX(wallX - (avatarCollisionBox.Left - (int)avatar.Position.X));
            }
            else
            {
                avatar.SetX(avatar.Position.X + deltaX);
            }
        }

        private static void ApplyRightMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes, float deltaX)
        {
            var avatarCollisionBox = avatar.GetCollisionBox();
            var wallX = GetRightWall(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Right == wallX)
            {
                avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                avatar.SetX((int)avatar.Position.X);
                return;
            }

            if (avatarCollisionBox.Right + deltaX >= wallX)
            {
                avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
                avatar.SetX((int)avatar.Position.X - (avatarCollisionBox.Right - wallX));
            }
            else
            {
                avatar.SetX(avatar.Position.X + deltaX);
            }
        }

        private static void ApplyUpwardMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarCollisionBox = avatar.GetCollisionBox();
            var ceilingY = GetCeiling(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Top == ceilingY)
            {
                avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                avatar.SetY((int)avatar.Position.Y);
                return;
            }

            var yDelta = Math.Min(avatar.Velocity.Y, AvatarConstants.MaxVelocity.Y);

            if (avatarCollisionBox.Top + yDelta <= ceilingY)
            {
                avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);
                avatar.SetY((int)avatar.Position.Y + (ceilingY - avatarCollisionBox.Top));
            }
            else
            {
                if (avatar.AvailableJumps == 1)
                {
                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.SetAnimation(Animation.Jumping);
                    }
                }
                else
                {
                    avatar.IncrementSpin();
                    if (avatar.Animation != Animation.Rolling)
                    {
                        avatar.SetAnimation(Animation.Spinning);
                    }
                }

                avatar.SetY(avatar.Position.Y + yDelta);
            }
        }

        private static void ApplyDownwardMotion(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarCollisionBox = avatar.GetCollisionBox();
            var floorY = GetFloor(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Bottom == floorY)
            {
                avatar.SetY((int)avatar.Position.Y);
                avatar.Grounded = true;
                avatar.AvailableJumps = 2;

                avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                avatar.Acceleration = new Vector2(avatar.Acceleration.X, 0);

                if (avatar.Animation == Animation.Falling)
                {
                    avatar.SetAnimation(Animation.Resting);
                }
                return;
            }

            var yDelta = Math.Min(avatar.Velocity.Y, AvatarConstants.MaxVelocity.Y);

            if (avatarCollisionBox.Bottom + yDelta >= floorY)
            {
                avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
                avatar.Grounded = true;
                avatar.SetY((int)avatar.Position.Y + (floorY - avatarCollisionBox.Bottom));

                if (avatar.Animation == Animation.Spinning || avatar.Animation != Animation.Rolling)
                {
                    avatar.SetBufferedAnimiation(Animation.Resting);
                }
            }
            else
            {
                avatar.SetY(avatar.Position.Y + yDelta);

                if (floorY - avatarCollisionBox.Bottom < 48)
                {
                    avatar.CloseToGround = true;
                }

                if (avatar.AvailableJumps == 2)
                {
                    avatar.AvailableJumps = 1;
                }

                if (!avatar.IsSpinning)
                {
                    avatar.SetAnimation(Animation.Falling);
                }
                else
                {
                    avatar.IncrementSpin();
                }
            }
        }

        private static void DecelerateX(Avatar avatar)
        {
            if (PhysicsHelper.FloatsAreEqual(avatar.Acceleration.X, 0))
            {
                avatar.Acceleration = new Vector2(0, avatar.Acceleration.Y);

                var decelerationConstant = avatar.Grounded ? AvatarConstants.RunningAcceleration : 4f;

                if (avatar.Animation == Animation.Rolling)
                {
                    decelerationConstant = 0.35f;
                }

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

        public static float GetLeftWall(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarLeftX = avatarCollisionBox.Left;

            var boxCandidates = sceneCollisionBoxes
                .Where(box => box.Right <= avatarLeftX)
                .Where(box => box.Height > CollisionHelper.PassableYThreshold)
                .Where(box => CollisionHelper.RectanglesIntersectInTheYPlane(avatarCollisionBox, box))
                .OrderByDescending(box => box.Right)
                .ToList();

            if (boxCandidates.Count > 0)
            {
                return boxCandidates.First().Right;
            }

            return 0;
        }

        public static float GetRightWall(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarRightX = avatarCollisionBox.Right;

            var boxCandidates = sceneCollisionBoxes
                .Where(box => box.Left >= avatarRightX)
                .Where(box => box.Height > CollisionHelper.PassableYThreshold)
                .Where(box => CollisionHelper.RectanglesIntersectInTheYPlane(avatarCollisionBox, box))
                .OrderBy(box => box.Left)
                .ToList();

            if (boxCandidates.Count > 0)
            {
                return boxCandidates.First().Left;
            }

            return 1920;
        }

        public static float GetCeiling(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarTopY = avatarCollisionBox.Top;

            var boxCandidates = sceneCollisionBoxes
                .Where(box => box.Bottom <= avatarTopY)
                .Where(box => box.Height > CollisionHelper.PassableYThreshold)
                .Where(box => CollisionHelper.RectanglesIntersectInTheXPlane(avatarCollisionBox, box))
                .OrderByDescending(box => box.Bottom)
                .ToList();

            if (boxCandidates.Count > 0)
            {
                return boxCandidates.First().Bottom;
            }

            return 0;
        }

        public static float GetFloor(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarBottomY = avatarCollisionBox.Bottom;
            var boxCandidates = sceneCollisionBoxes
                .Where(box => box.Top >= avatarBottomY)
                .Where(box => CollisionHelper.RectanglesIntersectInTheXPlane(avatarCollisionBox, box))
                .OrderBy(box => box.Top)
                .ToList();

            if (boxCandidates.Count > 0)
            {
                return boxCandidates.First().Top;
            }

            return 1080;
        }

        //public static void MoveIfIntersecting(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        //{
        //    var collisionBox = avatar.GetCollisionBox();

        //    var candidates = sceneCollisionBoxes.Where(box => collisionBox.Height > CollisionHelper.PassableYThreshold && box.Intersects(collisionBox));

        //    if (candidates.Count() > 1)
        //    {
        //        avatar.SetAnimation(Animation.Rolling);
        //    }
        //    else if (candidates.Count() == 1)
        //    {
        //        var box = candidates.First();

        //        var rightDifference = box.Right - collisionBox.Left;
        //        var leftDifference = collisionBox.Right - box.Left;
        //        var upDifference = collisionBox.Bottom - box.Top;
        //        var downDifference = box.Bottom - collisionBox.Top;

        //        List<int> differences = [rightDifference, leftDifference, upDifference, downDifference];

        //        differences = [.. differences.Where(difference => difference > 0).Order()];

        //        var smallestDifference = differences.First();

        //        if (rightDifference == smallestDifference)
        //        {
        //            avatar.SetX(avatar.Position.X + rightDifference);
        //            avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
        //        }
        //        else if (leftDifference == smallestDifference)
        //        {
        //            avatar.SetX(avatar.Position.X - leftDifference);
        //            avatar.Velocity = new Vector2(0, avatar.Velocity.Y);
        //        }
        //        else if (upDifference == smallestDifference)
        //        {
        //            avatar.SetY(avatar.Position.Y - upDifference);
        //            avatar.Velocity = new Vector2(avatar.Velocity.X, 0);

        //            avatar.Grounded = true;
        //        }
        //        else if (downDifference == smallestDifference)
        //        {
        //            avatar.SetY(avatar.Position.Y + downDifference);
        //            avatar.Velocity = new Vector2(avatar.Velocity.X, 0);
        //        }
        //    }

        //    var moreCandidates = sceneCollisionBoxes.Where(box => box.Intersects(avatar.GetCollisionBox()));
        //    if (moreCandidates.Any())
        //    {
        //        MoveIfIntersecting(avatar, sceneCollisionBoxes);
        //    }
        //}
    }
}
