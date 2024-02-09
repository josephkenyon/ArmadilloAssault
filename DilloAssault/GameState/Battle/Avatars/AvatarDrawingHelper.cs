using DilloAssault.Configuration;
using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Avatars
{
    public static class AvatarDrawingHelper
    {
        public static IDrawableObject GetBody(Avatar avatar)
        {
            return new Body(avatar);
        }

        private class Body(Avatar avatar) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;

            public TextureName TextureName => avatar.TextureName;

            public Rectangle GetDestinationRectangle() => new(
                (int)(avatar.Position.X + SpriteOffset + (avatar.IsSpinning ? (avatar.Size.X / 2) : 0)),
                (int)avatar.Position.Y + (avatar.IsSpinning ? (avatar.Size.Y / 2) : 0),
                avatar.Size.X,
                avatar.Size.Y
            );

            public Vector2 GetOrigin() => avatar.Origin;

            public float GetRotation() => avatar.IsSpinning ? avatar.Rotation : 0f;

            public Rectangle? GetSourceRectangle() => avatar.IsSpinning ? avatar.GetSourceRectangle() : new Rectangle(Point.Zero, avatar.Size);
        }

        public static IDrawableObject GetLeg(Avatar avatar, Direction whichLeg)
        {
            return new LegPart(avatar, whichLeg);
        }

        private class LegPart(Avatar avatar, Direction whichLeg) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;

            public TextureName TextureName => avatar.TextureName;

            public Rectangle GetDestinationRectangle() => new(
                (int)(avatar.Position.X + SpriteOffset),
                (int)avatar.Position.Y,
                avatar.Size.X,
                avatar.Size.Y
            );

            public Rectangle? GetSourceRectangle() {
                var sourceRectangle = avatar.GetSourceRectangle();

                if (whichLeg == Direction.Right)
                {
                    sourceRectangle.Y += avatar.Size.Y;
                }

                return sourceRectangle;
            }
        }

        public static IDrawableObject GetArm(Avatar avatar, Direction direction)
        {
            var spriteLocation = new Point(direction == Direction.Right ? 3 : 2, 0);
            return GetArm(avatar, spriteLocation);
        }

        public static IDrawableObject GetGun(Avatar avatar)
        {
            var weapon = avatar.SelectedWeapon;
            var textureName = ConfigurationManager.GetWeaponConfiguration(weapon.Type).TextureName;

            return GetArm(avatar, Point.Zero, textureName);
        }

        private static IDrawableObject GetArm(Avatar avatar, Point spriteLocation, TextureName? textureName = null)
        {
            var armOrigin = avatar.GetArmSpriteOrigin();
            return new ArmPart(avatar, armOrigin, spriteLocation, textureName: textureName, applyBreathing: true, applyRecoil: true);
        }

        public static IDrawableObject GetHead(Avatar avatar)
        {
            var spriteLocation = new Point(1, 0);
            var headOrigin = avatar.GetHeadOrigin();

            double xOffset = 0;
            double yOffset = -3;

            if (avatar.ArmAngle > 0f)
            {
                if (avatar.Direction == Direction.Right)
                {
                    xOffset = avatar.ArmAngle * 10;
                }
                else
                {
                    yOffset -= avatar.ArmAngle * 4;
                }
            }
            else if (avatar.ArmAngle < 0f)
            {
                if (avatar.Direction == Direction.Right)
                {
                    yOffset += avatar.ArmAngle * 4;
                }
                else
                {
                    xOffset = avatar.ArmAngle * 10;
                }
            }

            return new ArmPart(avatar, headOrigin, spriteLocation, offset: new Point((int)xOffset, (int)yOffset));
        }

        private class ArmPart(Avatar avatar, Vector2 origin, Point spriteLocation, TextureName? textureName = null, bool applyBreathing = false, Point? offset = null, bool applyRecoil = false) : IDrawableObject
        {
            private readonly int SpriteOffest = avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;

            public float GetRotation()
            {
                var rotation = (float)avatar.ArmAngle;

                if (applyRecoil)
                {
                    rotation += (avatar.Direction == Direction.Left) ? avatar.Recoil : -avatar.Recoil;
                }

                return rotation;
            }

            public Direction GetDirection() => avatar.Direction;

            public Vector2 GetOrigin() => origin;

            public TextureName TextureName => textureName != null ? (TextureName)textureName : avatar.TextureName;

            public Rectangle GetDestinationRectangle() => new(
                (int)(avatar.Position.X + origin.X + SpriteOffest + (offset != null ? ((Point)offset).X : 0)),
                (int)(avatar.Position.Y + origin.Y + (applyBreathing ? avatar.GetBreathingYOffset() : 0) + (offset != null ? ((Point)offset).Y : 0)),
                avatar.Size.X,
                avatar.Size.Y
            );

            public Rectangle? GetSourceRectangle() => new(spriteLocation.X * avatar.Size.X, spriteLocation.Y * avatar.Size.Y, avatar.Size.X, avatar.Size.Y);
        }
    }
}
