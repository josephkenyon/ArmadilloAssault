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

        public static void DrawCollection(ICollection<IDrawableObject> drawableObjects)
        {
            if (drawableObjects.Count > 0)
            {
                _spriteBatch.Begin();

                foreach (var drawableObject in drawableObjects)
                {
                    DrawObject(drawableObject);
                }

                _spriteBatch.End();
            }
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

        private static void DrawObject(IDrawableObject drawableObject)
        {
            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(drawableObject.TextureName),
                destinationRectangle: DrawingHelper.GetDestinationRectangle(drawableObject.Position),
                color: DrawingHelper.GetColor(drawableObject.Z),
                sourceRectangle: DrawingHelper.GetSourceRectangle(drawableObject.SpriteLocation)
            );
        }
    }
}
