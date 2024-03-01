using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArmadilloAssault.Graphics.Drawing.Avatars
{
    public static class AvatarDrawingHelper
    {
        private static Dictionary<AvatarType, Dictionary<Animation, AnimationJson>> Animations;

        public static void Initialize()
        {
            Animations = [];
            foreach (var avatarType in Enum.GetValues<AvatarType>())
            {
                var avatarJson = ConfigurationManager.GetAvatarConfiguration(avatarType);
                Animations.Add(avatarType, ConfigurationHelper.GetAnimations(avatarJson.Animations));
            }
        }

        public static ICollection<IDrawableObject> GetDrawableAvatars(AvatarFrame avatarFrame)
        {
            var drawableAvatars = new List<IDrawableAvatar>();

            var index = 0;
            foreach (var type in avatarFrame.Types)
            {
                try
                {
                    var drawableAvatar = new DrawableAvatar
                    {
                        Animation = avatarFrame.Animations[index],
                        ArmAngle = avatarFrame.ArmAngles[index],
                        AnimationFrame = avatarFrame.AnimationFrames[index],
                        BreathingYOffset = avatarFrame.BreathingYOffsets[index],
                        Dead = avatarFrame.Deads[index],
                        Direction = avatarFrame.Directions[index],
                        Position = avatarFrame.Positions[index],
                        Recoil = avatarFrame.Recoils[index],
                        Rotation = avatarFrame.Rotations[index],
                        Spinning = avatarFrame.Spinnings[index],
                        TextureName = avatarFrame.TextureNames[index],
                        Type = avatarFrame.Types[index],
                        WeaponTexture = avatarFrame.WeaponTextures[index],
                        Color = avatarFrame.Colors[index].ToColor(),
                        Opacity = avatarFrame.Colors[index].A / 255f
                    };

                    drawableAvatars.Add(drawableAvatar);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }

                index++;
            }

            return GetAvatars(drawableAvatars);
        }

        public static List<IDrawableObject> GetAvatars(ICollection<IDrawableAvatar> avatars)
        {
            var avatarCollection = new List<IDrawableObject>();

            var notSpinningOrDeadAvatars = avatars.Where(avatar => !avatar.Spinning && !avatar.Dead).ToList();

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetArm(avatar, Direction.Left)));

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetLeg(avatar, Direction.Left)));

            foreach (var avatar in avatars)
            {
                avatarCollection.Add(GetBody(avatar));
            }

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetLeg(avatar, Direction.Right)));

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetHead(avatar)));
            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetGun(avatar)));
            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(GetArm(avatar, Direction.Right)));

            return avatarCollection;
        }

        private static Body GetBody(IDrawableAvatar avatar) => new(avatar);

        private static float GetSpriteOffsetX(IDrawableAvatar avatar)
        {
            var offset = ConfigurationManager.GetAvatarConfiguration(avatar.Type).SpriteOffset.ToVector2();
            return avatar.Direction == Direction.Left ? -offset.X : offset.X;
        }

        private static Point GetSize(IDrawableAvatar avatar) => ConfigurationManager.GetAvatarConfiguration(avatar.Type).Size.ToPoint();

        private static Vector2 GetOrigin(IDrawableAvatar avatar)
        {
            var size = GetSize(avatar);
            return avatar.Spinning ? new Vector2(size.X / 2, size.Y / 2) : Vector2.Zero;
        }

        private static Rectangle GetSourceRectangle(IDrawableAvatar avatar)
        {
            var animation = Animations[avatar.Type][avatar.Animation];
            var size = GetSize(avatar);

            return new Rectangle()
            {
                X = (animation.X + avatar.AnimationFrame) * size.X,
                Y = animation.Y * size.Y,
                Width = size.X,
                Height = size.Y
            };
        }

        private class Body(IDrawableAvatar avatar) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => GetSpriteOffsetX(avatar);

            public TextureName Texture => avatar.TextureName;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + SpriteOffset + (avatar.Spinning ? size.X / 2 : 0)),
                    (int)avatar.Position.Y + (avatar.Spinning ? size.Y / 2 : 0),
                    size.X,
                    size.Y
                );
            }

            public Vector2 GetOrigin() => AvatarDrawingHelper.GetOrigin(avatar);

            public float GetRotation() => avatar.Spinning ? avatar.Rotation : 0f;

            public Rectangle? GetSourceRectangle() => avatar.Spinning || avatar.Dead ? AvatarDrawingHelper.GetSourceRectangle(avatar) : new Rectangle(Point.Zero, GetSize(avatar));

            public Color Color => avatar.Color;

            public float Opacity => avatar.Opacity;
        }

        private static LegPart GetLeg(IDrawableAvatar avatar, Direction whichLeg)
        {
            return new LegPart(avatar, whichLeg);
        }

        private class LegPart(IDrawableAvatar avatar, Direction whichLeg) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => GetSpriteOffsetX(avatar);

            public TextureName Texture => avatar.TextureName;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + SpriteOffset),
                    (int)avatar.Position.Y,
                    size.X,
                    size.Y
                );
            }

            public Rectangle? GetSourceRectangle()
            {
                var sourceRectangle = AvatarDrawingHelper.GetSourceRectangle(avatar);
                var size = GetSize(avatar);

                if (whichLeg == Direction.Right)
                {
                    sourceRectangle.Y += size.Y;
                }

                return sourceRectangle;
            }

            public Color Color => avatar.Color;

            public float Opacity => avatar.Opacity;
        }

        private static Limb GetArm(IDrawableAvatar avatar, Direction direction)
        {
            var spriteLocation = new Point(direction == Direction.Right ? 3 : 2, 0);
            return GetArm(avatar, spriteLocation);
        }

        private static Limb GetGun(IDrawableAvatar avatar)
        {
            var textureName = avatar.WeaponTexture;

            return GetArm(avatar, Point.Zero, textureName);
        }

        private static Vector2 GetArmSpriteOrigin(IDrawableAvatar avatar)
        {
            var avatarJson = ConfigurationManager.GetAvatarConfiguration(avatar.Type);

            var armOriginX = avatarJson.ArmOrigin.X;
            var armOriginY = avatarJson.ArmOrigin.Y;

            if (avatar.Direction == Direction.Left)
            {
                armOriginX += (avatarJson.Size.X / 2 - armOriginX) * 2;
            }

            return new Vector2(armOriginX, armOriginY);
        }

        private static Vector2 GetHeadOrigin(IDrawableAvatar avatar)
        {
            var avatarJson = ConfigurationManager.GetAvatarConfiguration(avatar.Type);

            var headOriginX = avatarJson.HeadOrigin.X;

            if (avatar.Direction == Direction.Left)
            {
                headOriginX += (avatarJson.Size.X / 2 - headOriginX) * 2;
            }

            return new Vector2(headOriginX, avatarJson.HeadOrigin.Y);
        }

        private static Limb GetArm(IDrawableAvatar avatar, Point spriteLocation, TextureName? textureName = null)
        {
            var armOrigin = GetArmSpriteOrigin(avatar);
            return new Limb(avatar, armOrigin, spriteLocation, textureName: textureName, applyBreathing: true, applyRecoil: true);
        }

        private static Limb GetHead(IDrawableAvatar avatar)
        {
            var spriteLocation = new Point(1, 0);
            var headOrigin = GetHeadOrigin(avatar);

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

            return new Limb(avatar, headOrigin, spriteLocation, offset: new Point((int)xOffset, (int)yOffset));
        }

        private class Limb(IDrawableAvatar avatar, Vector2 origin, Point spriteLocation, TextureName? textureName = null, bool applyBreathing = false, Point? offset = null, bool applyRecoil = false) : IDrawableObject
        {
            private readonly int SpriteOffest = (int)GetSpriteOffsetX(avatar);

            public float GetRotation()
            {
                var rotation = (float)avatar.ArmAngle;

                if (applyRecoil)
                {
                    rotation += avatar.Direction == Direction.Left ? avatar.Recoil : -avatar.Recoil;
                }

                return rotation;
            }

            public Direction GetDirection() => avatar.Direction;

            public Vector2 GetOrigin() => origin;

            public TextureName Texture => textureName != null ? (TextureName)textureName : avatar.TextureName;

            public Color Color => avatar.Color;

            public float Opacity => avatar.Opacity;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + origin.X + SpriteOffest + (offset != null ? ((Point)offset).X : 0)),
                    (int)(avatar.Position.Y + origin.Y + (applyBreathing ? avatar.BreathingYOffset : 0) + (offset != null ? ((Point)offset).Y : 0)),
                    size.X,
                    size.Y
                );
            }

            public Rectangle? GetSourceRectangle()
            {
                var size = GetSize(avatar);

                return new(spriteLocation.X * size.X, spriteLocation.Y * size.Y, size.X, size.Y);
            }
        }
    }
}
