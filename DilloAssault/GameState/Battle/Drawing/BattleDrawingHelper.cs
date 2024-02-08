using DilloAssault.Configuration;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.GameState.Battle.Environment.Clouds;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Drawing
{
    public static class BattleDrawingHelper
    {
        private static readonly Point ZeroPoint = Point.Zero;

        public static void DrawClouds()
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (var cloud in CloudManager.Clouds)
            {
                spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.clouds),
                    destinationRectangle: new Rectangle((int)cloud.Position.X, (int)cloud.Position.Y, cloud.Size.X, cloud.Size.Y),
                    sourceRectangle: new Rectangle(CloudManager.CloudSpriteSize * cloud.SpriteX, CloudManager.CloudSpriteSize * cloud.SpriteY, CloudManager.CloudSpriteSize, CloudManager.CloudSpriteSize),
                    color: Color.White * Math.Clamp(0.45f * Math.Abs(cloud.Speed), 0.2f, 1f)
                );
            }

            spriteBatch.End();
        }

        public static void DrawBullets()
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (var bullet in BulletManager.Bullets)
            {
                spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.bullet),
                    destinationRectangle: new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, 16, 4),
                    sourceRectangle: new Rectangle(0, 0, 118, 32),
                    color: Color.White,
                    rotation: bullet.Angle,
                    origin: Vector2.Zero,
                    effects: SpriteEffects.None,
                    1f
                );
            }

            spriteBatch.End();
        }

        public static void DrawEffects()
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();
            
            foreach (var effect in EffectManager.Effects)
            {
                var configuration = ConfigurationManager.GetEffectConfiguration(effect.Type.ToString());

                var spriteX = effect.FrameCounter % configuration.SpriteRowLength;
                var spriteY = effect.FrameCounter / configuration.SpriteRowLength;

                spriteBatch.Draw(
                    texture: TextureManager.GetTexture(configuration.TextureName),
                    destinationRectangle: new Rectangle((int)effect.Position.X, (int)effect.Position.Y, configuration.Size.X, configuration.Size.Y),
                    sourceRectangle: new Rectangle(spriteX * configuration.SpriteSize.X, spriteY * configuration.SpriteSize.Y, configuration.SpriteSize.X, configuration.SpriteSize.Y),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    effects: effect.Direction == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    1f
                );
            }

            spriteBatch.End();
        }

        public static void DrawAvatars(ICollection<Avatar> avatars)
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (var avatar in avatars)
            {
                if (!avatar.IsSpinning) {
                    DrawAvatarArm(spriteBatch, avatar, Direction.Left);
                }

                DrawAvatarBody(spriteBatch, avatar);

                if (!avatar.IsSpinning)
                {
                    DrawAvatarHead(spriteBatch, avatar);
                    DrawAvatarGun(spriteBatch, avatar);
                    DrawAvatarArm(spriteBatch, avatar, Direction.Right);
                }
            }

            spriteBatch.End();
        }

        private static void DrawAvatarBody(SpriteBatch spriteBatch, Avatar avatar)
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

            // Draw left leg, or body if spinning
            spriteBatch.Draw(
                texture: TextureManager.GetTexture(avatar.TextureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
                color: Color.White,
                rotation: isSpinning ? (flipDirection ? -avatar.SpinningAngle : avatar.SpinningAngle) : 0f,
                origin: isSpinning ? new Vector2(64, 64) : Vector2.Zero,
                flipDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f
            );

            if (!isSpinning)
            {
                // Draw body
                spriteBatch.Draw(
                   texture: TextureManager.GetTexture(avatar.TextureName),
                   destinationRectangle: destinationRectangle,
                   sourceRectangle: new Rectangle(Point.Zero, avatar.Size),
                   color: Color.White,
                   rotation: isSpinning ? (flipDirection ? -avatar.SpinningAngle : avatar.SpinningAngle) : 0f,
                   origin: isSpinning ? new Vector2(64, 64) : Vector2.Zero,
                   flipDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                   1f
                );

                sourceRectangle = new Rectangle(sourceRectangle.X, sourceRectangle.Y + (1 * avatar.Size.Y), sourceRectangle.Width, sourceRectangle.Height);

                // Draw right leg
                spriteBatch.Draw(
                   texture: TextureManager.GetTexture(avatar.TextureName),
                   destinationRectangle: destinationRectangle,
                   sourceRectangle: sourceRectangle,
                   color: Color.White,
                   rotation: isSpinning ? (flipDirection ? -avatar.SpinningAngle : avatar.SpinningAngle) : 0f,
                   origin: isSpinning ? new Vector2(64, 64) : Vector2.Zero,
                   flipDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                   1f
                );
            }
        }

        private static void DrawAvatarArm(SpriteBatch spriteBatch, Avatar avatar, Direction direction)
        {
            var spriteLocation = new Point(direction == Direction.Right ? 3 : 2, 0);
            DrawAvatarArm(spriteBatch, avatar, spriteLocation, avatar.TextureName);
        }

        private static void DrawAvatarGun(SpriteBatch spriteBatch, Avatar avatar)
        {
            var weapon = avatar.SelectedWeapon;
            var textureName = ConfigurationManager.GetWeaponConfiguration(weapon.Type.ToString()).TextureName;

            DrawAvatarArm(spriteBatch, avatar, Point.Zero, textureName);
        }

        private static void DrawAvatarArm(SpriteBatch spriteBatch, Avatar avatar, Point spriteLocation, TextureName textureName)
        {
            var armOrigin = avatar.GetArmSpriteOrigin();
            DrawAvatarBodyPart(spriteBatch, avatar, armOrigin, spriteLocation, textureName, true, applyRecoil: true);
        }
        private static void DrawAvatarHead(SpriteBatch spriteBatch, Avatar avatar)
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

            DrawAvatarBodyPart(spriteBatch, avatar, headOrigin, spriteLocation, avatar.TextureName, offset: new Point((int)xOffset, (int)yOffset));
        }

        private static void DrawAvatarBodyPart(SpriteBatch spriteBatch, Avatar avatar, Vector2 origin, Point spriteLocation, TextureName textureName, bool applyBreathing = false, Point? offset = null, bool applyRecoil = false)
        {
            var spriteOffset = avatar.Direction == Direction.Left ? -avatar.SpriteOffset.X : avatar.SpriteOffset.X;

            var destinationRectangle = new Rectangle(
               (int)(avatar.Position.X + origin.X + spriteOffset + (offset != null ? ((Point)offset).X : 0)),
               (int)(avatar.Position.Y + origin.Y + (applyBreathing ? avatar.GetBreathingYOffset() : 0) + (offset != null ? ((Point)offset).Y : 0)),
               avatar.Size.X,
               avatar.Size.Y
            );

            var rotation = (float)avatar.ArmAngle;

            if (applyRecoil)
            {
                rotation += (avatar.Direction == Direction.Left) ? avatar.Recoil : -avatar.Recoil;
            }

            spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: new Rectangle(spriteLocation.X * avatar.Size.X, spriteLocation.Y * avatar.Size.Y, avatar.Size.X, avatar.Size.Y),
                Color.White,
                rotation: rotation,
                origin,
                avatar.Direction == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f);
        }
    }
}
