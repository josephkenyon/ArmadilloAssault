using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DilloAssault.Graphics.Drawing
{
    static class DrawingManager
    {
        private static SpriteBatch _spriteBatch;

        public static void LoadContent(GraphicsDevice graphicsDevice) {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public static void DrawCollection(IEnumerable<IDrawableObject> drawableObjects)
        {
            _spriteBatch.Begin();

            foreach (var drawableObject in drawableObjects)
            {
                DrawObject(drawableObject);
            }

            _spriteBatch.End();
        }

        private static void DrawObject(IDrawableObject drawableObject)
        {
            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(drawableObject.TextureName),
                destinationRectangle: DrawingHelper.GetDestinationRectangle(drawableObject.Position),
                color: DrawingHelper.GetColor(drawableObject.Z),
                sourceRectangle: DrawingHelper.GetSourceRectangle(drawableObject.SpriteLocation)
            );
        }

        public static void DrawTexture(TextureName textureName, Point position)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                position: position.ToVector2() * DrawingHelper.TileSize,
                color: Color.White
            );

            _spriteBatch.End();
        }

        public static void DrawString(string text, Point position, SpriteFont spriteFont)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(spriteFont, text, position.ToVector2() * DrawingHelper.TileSize, Color.White);
            _spriteBatch.End();
        }

        public static void DrawAvatarBackground(Avatar avatar)
        {
            _spriteBatch.Begin();

            var flipDirection = avatar.Direction == Direction.Left;
            var isSpinning = avatar.IsSpinning;

            var offset = isSpinning ? avatar.Size.X / 2 : 0;

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(avatar.SpriteTextureName),
                destinationRectangle: new Rectangle((int)avatar.Position.X + offset, (int)avatar.Position.Y + offset, avatar.Size.X, avatar.Size.Y),
                sourceRectangle: avatar.GetSourceRectangle(),
                color: Color.White,
                rotation: isSpinning ? (flipDirection ? -avatar.SpinningAngle : avatar.SpinningAngle) : 0f,
                origin: isSpinning ? new Vector2(64, 64) : Vector2.Zero,
                flipDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f
            );

            _spriteBatch.End();
        }

        public static void DrawAvatarForeground(Avatar avatar)
        {
            if (!avatar.IsSpinning)
            {
                _spriteBatch.Begin();

                var armOrigin = avatar.GetArmOrigin();

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(avatar.ArmTextureName),
                    destinationRectangle: new Rectangle((int)(avatar.Position.X + armOrigin.X), (int)(avatar.Position.Y + armOrigin.Y), avatar.Size.X, avatar.Size.Y),
                    null,
                    Color.White,
                    (float)avatar.AimAngle,
                    avatar.GetArmOrigin(),
                    avatar.Direction == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    1f);

                _spriteBatch.End();
            }
        }

        public static void DrawCollisionBoxes(IEnumerable<Rectangle> rectangles)
        {
            _spriteBatch.Begin();

            var scaleConstant = DrawingHelper.ScaleConstant;

            foreach(var rectangle in rectangles)
            {
                var destinationRectangle = new Rectangle((int)(rectangle.X * scaleConstant), (int)(rectangle.Y * scaleConstant), (int)(rectangle.Width * scaleConstant), (int)(rectangle.Height * scaleConstant));

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: Color.Yellow * 0.35f
                );
            }

            _spriteBatch.End();
        }
    }
}
