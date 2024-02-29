using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Sound;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Physics
{
    public static class PhysicsManager
    {
        private static readonly float gravityAcceleration = 0.035f;
         
        public static void Update(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes)
        {
            ApplyHorizontalMotion(physicsObject, sceneCollisionBoxes);
            ApplyVerticalMotion(physicsObject, sceneCollisionBoxes);

            var collisionBox = physicsObject.GetCollisionBox();
            if (collisionBox.Left >= 1920 - 1)
            {
                physicsObject.SetX(physicsObject.Position.X - 1920 - collisionBox.Width + 1);
            }
            else if (collisionBox.Right <= 1)
            {
                physicsObject.SetX(physicsObject.Position.X + 1920 + collisionBox.Width - 1);
            }

            if (collisionBox.Bottom < 0)
            {
                physicsObject.SetY(physicsObject.Position.Y + 1080 + collisionBox.Height - 1);
            }
            else if (collisionBox.Top > 1080)
            {
                physicsObject.SetY(physicsObject.Position.Y - 1080 - collisionBox.Height + 1);
            }
        }

        public static void Update(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
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

            Update(avatar as PhysicsObject, sceneCollisionBoxes);
        }


        private static bool CanExitSpinning(Avatar avatar, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var collisionBox = avatar.GetCollisionBox();
            var ceiling = GetCeiling(collisionBox, sceneCollisionBoxes);

            return collisionBox.Top - ceiling > 64;
        }

        private static void ApplyHorizontalMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes)
        {
            physicsObject.Velocity = new Vector2(
                Math.Clamp(
                    physicsObject.Velocity.X + physicsObject.Acceleration.X,
                    -physicsObject.MaxVelocity.X, physicsObject.MaxVelocity.X
                ),
                physicsObject.Velocity.Y
            );

            var xVelocity = physicsObject.Velocity.X + physicsObject.RunningVelocity + physicsObject.InfluenceVelocity;

            if (xVelocity > 0)
            {
                ApplyRightMotion(physicsObject, sceneCollisionBoxes, xVelocity);
            }
            else if (xVelocity < 0)
            {
                ApplyLeftMotion(physicsObject, sceneCollisionBoxes, xVelocity);
            }

            DecelerateX(physicsObject);
        }

        private static void ApplyVerticalMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes)
        {
            physicsObject.Velocity = new Vector2(
                physicsObject.Velocity.X,
                Math.Clamp(
                    physicsObject.Velocity.Y + physicsObject.Acceleration.Y,
                    -physicsObject.MaxVelocity.Y, physicsObject.MaxVelocity.Y
                )
            );

            if (physicsObject.Velocity.Y < 0)
            {
                ApplyUpwardMotion(physicsObject, sceneCollisionBoxes);
            }
            else
            {
                ApplyDownwardMotion(physicsObject, sceneCollisionBoxes);
            }

            DecelerateY(physicsObject);
        }

        private static void ApplyLeftMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes, float deltaX)
        {
            var collisionBox = physicsObject.GetCollisionBox();
            var wallX = GetLeftWall(collisionBox, sceneCollisionBoxes);

            if (collisionBox.Left == wallX)
            {
                physicsObject.SetX((int)physicsObject.Position.X);
                physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                return;
            }

            if (collisionBox.Left + deltaX <= wallX)
            {
                physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                physicsObject.SetX(wallX - (collisionBox.Left - (int)physicsObject.Position.X));
            }
            else
            {
                physicsObject.SetX(physicsObject.Position.X + deltaX);
            }
        }

        private static void ApplyRightMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes, float deltaX)
        {
            var avatarCollisionBox = physicsObject.GetCollisionBox();
            var wallX = GetRightWall(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Right == wallX)
            {
                physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                physicsObject.SetX((int)physicsObject.Position.X);
                return;
            }

            if (avatarCollisionBox.Right + deltaX >= wallX)
            {
                physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                physicsObject.SetX((int)physicsObject.Position.X - (avatarCollisionBox.Right - wallX));
            }
            else
            {
                physicsObject.SetX(physicsObject.Position.X + deltaX);
            }
        }

        private static void ApplyUpwardMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarCollisionBox = physicsObject.GetCollisionBox();
            var ceilingY = GetCeiling(avatarCollisionBox, sceneCollisionBoxes);

            if (avatarCollisionBox.Top == ceilingY)
            {
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                physicsObject.Acceleration = new Vector2(physicsObject.Acceleration.X, 0);
                physicsObject.SetY((int)physicsObject.Position.Y);
                return;
            }

            var yDelta = Math.Min(physicsObject.Velocity.Y, physicsObject.MaxVelocity.Y);

            if (avatarCollisionBox.Top + yDelta <= ceilingY)
            {
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                physicsObject.Acceleration = new Vector2(physicsObject.Acceleration.X, 0);
                physicsObject.SetY((int)physicsObject.Position.Y + (ceilingY - avatarCollisionBox.Top));
            }
            else
            {
                physicsObject.Rising = true;
                physicsObject.SetY(physicsObject.Position.Y + yDelta);
            }
        }

        private static void ApplyDownwardMotion(PhysicsObject physicsObject, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var collisionBox = physicsObject.GetCollisionBox();
            var floor = GetFloor(collisionBox, sceneCollisionBoxes);
            var floorY = floor.Value;

            if (collisionBox.Bottom == floorY)
            {
                if (physicsObject is Avatar)
                {
                    var avatar = physicsObject as Avatar;

                    if (avatar.DropThrough)
                    {
                        if (floor.Passable)
                        {
                            avatar.SetY(avatar.Position.Y + 1);
                        }
                        else
                        {
                            avatar.SetAnimation(Animation.Rolling);
                        }

                        avatar.DropThrough = false;
                        return;
                    }
                }

                physicsObject.SetY((int)physicsObject.Position.Y);
                physicsObject.Grounded = true;

                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                physicsObject.Acceleration = new Vector2(physicsObject.Acceleration.X, 0);
                return;
            }

            var yDelta = Math.Min(physicsObject.Velocity.Y, physicsObject.MaxVelocity.Y);

            if (collisionBox.Bottom + yDelta >= floorY)
            {
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                physicsObject.Grounded = true;
                if (physicsObject is Avatar)
                {
                    var avatar = physicsObject as Avatar;
                    if (avatar.Animation == Animation.Rolling)
                    {
                        SoundManager.QueueBattleSound(BattleSound.rolling_grass);
                    }
                    else
                    {
                        SoundManager.QueueBattleSound(BattleSound.footstep_grass);
                    }
                }
                physicsObject.SetY((int)physicsObject.Position.Y + (floorY - collisionBox.Bottom));
            }
            else
            {
                physicsObject.SetY(physicsObject.Position.Y + yDelta);

                if (floorY - collisionBox.Bottom < 48)
                {
                    physicsObject.CloseToGround = true;
                }

                physicsObject.Falling = true;
                physicsObject.Rising = false;
            }
        }

        private static void DecelerateX(PhysicsObject physicsObject)
        {
            if (MathUtils.FloatsAreEqual(physicsObject.Acceleration.X, 0))
            {
                physicsObject.Acceleration = new Vector2(0, physicsObject.Acceleration.Y);

                var decelerationConstant = physicsObject.Grounded ? Avatar.RunningAcceleration : 4f;

                if (physicsObject.LowDrag && !physicsObject.Grounded)
                {
                    decelerationConstant = 0.25f;
                }

                if (physicsObject.Velocity.X > 0)
                {
                    var newVelocityX = Math.Max(0, physicsObject.Velocity.X - decelerationConstant);
                    physicsObject.Velocity = new Vector2(newVelocityX, physicsObject.Velocity.Y);
                }
                else if (physicsObject.Velocity.X < 0)
                {
                    var newVelocityX = Math.Min(0, physicsObject.Velocity.X + decelerationConstant);
                    physicsObject.Velocity = new Vector2(newVelocityX, physicsObject.Velocity.Y);
                }
            }
        }

        private static void DecelerateY(PhysicsObject physicsObject)
        {
            if (MathUtils.FloatsAreEqual(physicsObject.Acceleration.Y, 0) || physicsObject.Grounded)
            {
                physicsObject.Acceleration = new Vector2(physicsObject.Acceleration.X, 0);
            }

            if (!physicsObject.Grounded)
            {
                physicsObject.Acceleration = new Vector2(physicsObject.Acceleration.X, physicsObject.Acceleration.Y + gravityAcceleration);
            }
        }

        private static float GetLeftWall(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
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

            return -9999;
        }

        private static float GetRightWall(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
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

            return 9999;
        }

        private static float GetCeiling(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
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

            return -9999;
        }

        private static Floor GetFloor(Rectangle avatarCollisionBox, ICollection<Rectangle> sceneCollisionBoxes)
        {
            var avatarBottomY = avatarCollisionBox.Bottom;
            var boxCandidates = sceneCollisionBoxes
                .Where(box => box.Top >= avatarBottomY)
                .Where(box => CollisionHelper.RectanglesIntersectInTheXPlane(avatarCollisionBox, box))
                .OrderBy(box => box.Top)
                .ToList();

            if (boxCandidates.Count > 1)
            {
                var passable = boxCandidates.First().Height < CollisionHelper.PassableYThreshold;

                if (boxCandidates[1].Top - boxCandidates[0].Top <= CollisionHelper.PassableYThreshold)
                {
                    passable = false;
                }

                return new Floor { Value = boxCandidates.First().Top, Passable = passable };
            }
            else if (boxCandidates.Count > 0)
            {
                return new Floor { Value = boxCandidates.First().Top, Passable = boxCandidates.First().Height < CollisionHelper.PassableYThreshold };
            }

            return new Floor{Value = 9999, Passable = false };
        }

        private class Floor
        {
            public bool Passable { get; set; }
            public float Value { get; set; }
        }
    }
}
