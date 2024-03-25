using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public static ICollection<IDrawableObject> GetDrawableAvatars(LobbyAvatarFrame lobbyAvatarFrame, float breathingOffset)
        {
            var drawableAvatars = new List<IDrawableAvatar>();

            var positions = lobbyAvatarFrame.GetPositions();

            var index = 0;
            foreach (var type in lobbyAvatarFrame.Types)
            {
                try
                {
                    var drawableAvatar = new DrawableAvatar
                    {
                        Animation = Animation.Resting,
                        ArmAngle = 0f,
                        AnimationFrame = 0,
                        BreathingYOffset = breathingOffset,
                        Dead = false,
                        Crowned = lobbyAvatarFrame.Crowneds[index],
                        Direction = Direction.Right,
                        Position = positions[index],
                        Recoil = 0f,
                        Rotation = 0f,
                        Spinning = false,
                        TextureName = Avatar.GetTextureName(type),
                        WhiteTextureName = Avatar.GetTextureName(type),
                        Type = type,
                        WeaponTexture = TextureName.pistol,
                        Color = Color.White,
                        TeamColor = null,
                        Opacity = 1f
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

        public static ICollection<IDrawableObject> GetDrawableAvatars(AvatarFrame avatarFrame, AvatarStaticData avatarStaticData, int playerIndex = 0)
        {
            var drawableAvatars = new List<IDrawableAvatar>();

            if (avatarStaticData.PlayerIndices.Count > 0)
            {
                var playerAvatarIndex = avatarStaticData.PlayerIndices.FindIndex(frame => frame == playerIndex);
                var playerTeamIndex = playerAvatarIndex != -1 ? avatarStaticData.TeamIndices[playerAvatarIndex] : 0;

                var positions = avatarFrame.GetPositions();

                var index = 0;
                foreach (var type in avatarStaticData.Types)
                {
                    try
                    {
                        var avatarPlayerIndex = avatarStaticData.PlayerIndices[index];
                        var avatarTeamIndex = avatarStaticData.TeamIndices[index];

                        var drawableAvatar = new DrawableAvatar
                        {
                            Animation = avatarFrame.Animations[index],
                            ArmAngle = avatarFrame.ArmAngles[index],
                            AnimationFrame = avatarFrame.AnimationFrames[index],
                            BreathingYOffset = avatarFrame.BreathingYOffsets[index],
                            Dead = avatarFrame.Animations[index] == Animation.Dead,
                            Crowned = avatarStaticData.HasCrowns[index],
                            Direction = avatarFrame.Directions[index],
                            Position = positions[index],
                            Recoil = avatarFrame.Recoils[index],
                            Rotation = avatarFrame.Rotations[index],
                            Spinning = avatarFrame.Animations[index] == Animation.Rolling || avatarFrame.Animations[index] == Animation.Spinning,
                            TextureName = avatarStaticData.TextureNames[index],
                            WhiteTextureName = avatarStaticData.WhiteTextureNames[index],
                            Type = avatarStaticData.Types[index],
                            WeaponTexture = avatarFrame.WeaponTextures[index],
                            Color = avatarFrame.Colors[index].ToColor(),
                            TeamColor = avatarStaticData.ShowTeamColors[index] ? DrawingHelper.GetTeamColor(avatarStaticData.TeamIndices[index]) : null,
                            Opacity = MathUtils.GetAlpha(avatarFrame.Invisibles[index], playerTeamIndex, avatarTeamIndex)
                        };

                        drawableAvatars.Add(drawableAvatar);
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(ex);
                    }

                    index++;
                }

            }

            return GetAvatars(drawableAvatars);
        }

        public static List<IDrawableObject> GetAvatars(ICollection<IDrawableAvatar> avatars)
        {
            var avatarCollection = new List<IDrawableObject>();

            foreach (var avatar in avatars)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0 && avatar.TeamColor == null)
                    {
                        continue;
                    }

                    if (!avatar.Spinning && !avatar.Dead)
                    {
                        avatarCollection.Add(GetArm(avatar, Direction.Left, i == 0));
                        avatarCollection.Add(GetLeg(avatar, Direction.Left, i == 0));
                    }

                    avatarCollection.Add(GetBody(avatar, i == 0));

                    if (avatar.Spinning && i == 1 && avatar.Crowned)
                    {
                        avatarCollection.Add(GetBody(avatar, i == 0, true));
                    }

                    if (!avatar.Spinning && !avatar.Dead)
                    {
                        avatarCollection.Add(GetLeg(avatar, Direction.Right, i == 0));

                        avatarCollection.Add(GetHead(avatar, true, i == 0));
                        avatarCollection.Add(GetHead(avatar, false, i == 0));

                        avatarCollection.Add(GetGun(avatar));
                        avatarCollection.Add(GetArm(avatar, Direction.Right, i == 0));

                        if (avatar.Crowned && i == 1)
                        {
                            avatarCollection.Add(GetHead(avatar, false, false, true));
                        }
                    }
                }
            }

            return avatarCollection;
        }

        private static Body GetBody(IDrawableAvatar avatar, bool white = false, bool crown = false) => new(avatar, white, crown);

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

        private static Rectangle GetSourceRectangle(IDrawableAvatar avatar, bool crown = false)
        {
            var animation = Animations[avatar.Type][avatar.Animation];
            var size = GetSize(avatar);

            return new Rectangle()
            {
                X = (crown ? 8 : animation.X + avatar.AnimationFrame) * size.X,
                Y = animation.Y * size.Y,
                Width = avatar.Spinning ? (size.X - 3) : size.X,
                Height = size.Y
            };
        }

        private class Body(IDrawableAvatar avatar, bool white = false, bool crown = false) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => GetSpriteOffsetX(avatar);

            public TextureName Texture => white ? avatar.WhiteTextureName : avatar.TextureName;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + SpriteOffset + (avatar.Spinning ? size.X / 2 : 0)) - CameraManager.Offset.X,
                    (int)avatar.Position.Y + (white && avatar.Dead ? 3 : 0) + (avatar.Spinning ? size.Y / 2 : 0) - CameraManager.Offset.Y,
                    size.X,
                    size.Y
                );
            }

            public Vector2 GetOrigin() => AvatarDrawingHelper.GetOrigin(avatar);

            public float GetRotation() => avatar.Spinning ? avatar.Rotation : 0f;

            public Rectangle? GetSourceRectangle() => avatar.Spinning || avatar.Dead ? AvatarDrawingHelper.GetSourceRectangle(avatar, crown) : new Rectangle(Point.Zero, GetSize(avatar));

            public Color Color => white ? (Color)avatar.TeamColor : avatar.Color;

            public float Opacity => avatar.Opacity;
        }

        private static LegPart GetLeg(IDrawableAvatar avatar, Direction whichLeg, bool white = false)
        {
            return new LegPart(avatar, whichLeg, white);
        }

        private class LegPart(IDrawableAvatar avatar, Direction whichLeg, bool white = false) : IDrawableObject
        {
            public Direction GetDirection() => avatar.Direction;

            private float SpriteOffset => GetSpriteOffsetX(avatar);

            public TextureName Texture => white ? avatar.WhiteTextureName : avatar.TextureName;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + SpriteOffset) - CameraManager.Offset.X,
                    (int)avatar.Position.Y + (white ? 3 : 0) - CameraManager.Offset.Y,
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

            public Color Color => white ? (Color)avatar.TeamColor : avatar.Color;

            public float Opacity => avatar.Opacity;
        }

        private static Limb GetArm(IDrawableAvatar avatar, Direction direction, bool white = false)
        {
            var spriteLocation = new Point(direction == Direction.Right ? 3 : 2, 0);
            return GetArm(avatar, spriteLocation, null, white);
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

        private static Limb GetArm(IDrawableAvatar avatar, Point spriteLocation, TextureName? textureName = null, bool white = false)
        {
            var armOrigin = GetArmSpriteOrigin(avatar);
            return new Limb(avatar, armOrigin, spriteLocation, textureName: textureName, applyBreathing: true, applyRecoil: true, white: white);
        }

        private static Limb GetHead(IDrawableAvatar avatar, bool background, bool white = false, bool crown = false)
        {
            var spriteLocation = new Point(background ? 6 : (crown ? 7 : 1), 0);
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

            return new Limb(avatar, headOrigin, spriteLocation, offset: new Point((int)xOffset, (int)yOffset), white: white);
        }

        private class Limb(IDrawableAvatar avatar, Vector2 origin, Point spriteLocation, TextureName? textureName = null,
            bool applyBreathing = false, Point? offset = null, bool applyRecoil = false, bool white = false) : IDrawableObject
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

            public TextureName Texture => white ? avatar.WhiteTextureName : (textureName != null ? (TextureName)textureName : avatar.TextureName);

            public Color Color => white ? (Color)avatar.TeamColor : avatar.Color;

            public float Opacity => avatar.Opacity;

            public Rectangle GetDestinationRectangle()
            {
                var size = GetSize(avatar);

                return new(
                    (int)(avatar.Position.X + origin.X + SpriteOffest + (offset != null ? ((Point)offset).X : 0)) - CameraManager.Offset.X,
                    (int)(avatar.Position.Y + origin.Y + (applyBreathing ? avatar.BreathingYOffset : 0) + (offset != null ? ((Point)offset).Y : 0)) - CameraManager.Offset.Y,
                    size.X,
                    size.Y
                );
            }

            public Rectangle? GetSourceRectangle()
            {
                var size = GetSize(avatar);

                return new(spriteLocation.X * size.X + 3, spriteLocation.Y * size.Y, size.X - 6, size.Y);
            }
        }
    }
}
