using DilloAssault.Configuration;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.GameState.Battle.Weapons;
using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Drawing
{
    public static class BattleDrawingHelper
    {
        public static void DrawAvatars(ICollection<Avatar> avatars)
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (var avatar in avatars.Where(avatar => !avatar.IsSpinning))
            {
                DrawAvatarArm(spriteBatch, avatar, Direction.Left);
            }

            foreach (var avatar in avatars)
            {
                DrawAvatarBackground(spriteBatch, avatar);
            }

            foreach (var avatar in avatars.Where(avatar => !avatar.IsSpinning))
            {
                DrawAvatarHead(spriteBatch, avatar);
                DrawAvatarGun(spriteBatch, avatar);
                DrawAvatarArm(spriteBatch, avatar, Direction.Right);
            }

            spriteBatch.End();
        }

        private static void DrawAvatarBackground(SpriteBatch spriteBatch, Avatar avatar)
        {

            var flipDirection = avatar.Direction == Direction.Left;
            var isSpinning = avatar.IsSpinning;

            var sourceRectangle = avatar.GetSourceRectangle();
            if (!isSpinning)
            {
                sourceRectangle = new Rectangle(sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height);
            }

            var offset = isSpinning ? avatar.Size.X / 2 : 0;

            var destinationRectangle = new Rectangle((int)avatar.Position.X + offset, (int)avatar.Position.Y + offset, avatar.Size.X, avatar.Size.Y);
            if (!isSpinning)
            {
                var spriteOffset = avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;
                destinationRectangle = new Rectangle(destinationRectangle.X + spriteOffset, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            }

            spriteBatch.Draw(
                texture: TextureManager.GetTexture(avatar.SpriteTextureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
                color: Color.White,
                rotation: isSpinning ? (flipDirection ? -avatar.SpinningAngle : avatar.SpinningAngle) : 0f,
                origin: isSpinning ? new Vector2(64, 64) : Vector2.Zero,
                flipDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f
            );
        }

        private static void DrawAvatarArm(SpriteBatch spriteBatch, Avatar avatar, Direction direction)
        {
            var textureName = direction == Direction.Right ? avatar.RightArmTextureName : avatar.LeftArmTextureName;
            DrawAvatarArm(spriteBatch, avatar, textureName);
        }

        private static void DrawAvatarGun(SpriteBatch spriteBatch, Avatar avatar)
        {
            var weapon = avatar.SelectedWeapon;
            var textureName = ConfigurationManager.GetWeaponConfiguration(weapon.Type.ToString()).TextureName;

            DrawAvatarArm(spriteBatch, avatar, textureName);
        }

        private static void DrawAvatarArm(SpriteBatch spriteBatch, Avatar avatar, TextureName textureName)
        {
            var armOrigin = avatar.GetArmOrigin();
            DrawAvatarBodyPart(spriteBatch, avatar, armOrigin, textureName, true);
        }
        private static void DrawAvatarHead(SpriteBatch spriteBatch, Avatar avatar)
        {
            var headOrigin = avatar.GetHeadOrigin();

            double xOffset = 0;
            double yOffset = -3;

            if (avatar.AimAngle > 0f)
            {
                if (avatar.Direction == Direction.Right)
                {
                    xOffset = avatar.AimAngle * 7;
                }
                else
                {
                    yOffset -= avatar.AimAngle * 3;
                }
            }
            else if (avatar.AimAngle < 0f)
            {
                if (avatar.Direction == Direction.Right)
                {
                    yOffset += avatar.AimAngle * 3;
                }
                else
                {
                    xOffset = avatar.AimAngle * 7;
                }
            }

            DrawAvatarBodyPart(spriteBatch, avatar, headOrigin, avatar.HeadTextureName, offset: new Point((int)xOffset, (int)yOffset));
        }

        private static void DrawAvatarBodyPart(SpriteBatch spriteBatch, Avatar avatar, Vector2 origin, TextureName textureName, bool applyBreathing = false, Point? offset = null)
        {
            var spriteOffset = avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;

            var destinationRectangle = new Rectangle(
               (int)(avatar.Position.X + origin.X + spriteOffset + (offset != null ? ((Point)offset).X : 0)),
               (int)(avatar.Position.Y + origin.Y + (applyBreathing ? avatar.GetBreathingYOffset() : 0) + (offset != null ? ((Point)offset).Y : 0)),
               avatar.Size.X,
               avatar.Size.Y
            );

            spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                destinationRectangle: destinationRectangle,
                null,
                Color.White,
                (float)avatar.AimAngle,
                origin,
                avatar.Direction == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f);
        }
    }
}
