using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing.Textures;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ArmadilloAssault.Graphics.Drawing
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
                destinationRectangle: drawableObject.GetDestinationRectangle(),
                sourceRectangle: drawableObject.GetSourceRectangle(),
                color: DrawingHelper.GetColor(drawableObject.Z) * drawableObject.Opacity,
                rotation: drawableObject.GetRotation(),
                origin: drawableObject.GetOrigin(),
                effects: drawableObject.GetDirection() == Direction.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: drawableObject.LayerDepth
            );
        }

        public static void DrawTexture(TextureName textureName, Rectangle destinationRectangle, float opacity = 1f)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: null,
                rotation: 0f,
                effects: SpriteEffects.None,
                origin: Vector2.Zero,
                color: Color.White * opacity,
                layerDepth: 1f
            );

            _spriteBatch.End();
        }

        public static void DrawTexture(TextureName textureName, Vector2 position)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                position: position,
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

        public static void DrawMenuButtons(IEnumerable<ButtonJson> buttons)
        {
            _spriteBatch.Begin();

            foreach (var button in buttons)
            {
                var destinationRectangle = button.GetRectangle();

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: Color.Red * 0.5f
                );

                var font = DrawingHelper.GetFont;
                var size = font.MeasureString(button.Text);

                var textPosition = destinationRectangle.Center - (size / 2).ToPoint();

                _spriteBatch.DrawString(
                    font, button.Text, textPosition.ToVector2(), Color.White
                );
            }

            _spriteBatch.End();
        }

        public static void DrawHud(HudFrame hudFrame)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(TextureName.white_pixel),
                destinationRectangle: new Rectangle(hudFrame.AvatarX + 16, hudFrame.AvatarY - 24, 96, 8),
                sourceRectangle: new Rectangle(0, 0, 1, 1),
                color: Color.Black * 0.5f
            );

            _spriteBatch.Draw(
               texture: TextureManager.GetTexture(TextureName.white_pixel),
               destinationRectangle: new Rectangle(hudFrame.AvatarX + 16, hudFrame.AvatarY - 24, 96 * (hudFrame.Health / 100), 8),
               sourceRectangle: new Rectangle(0, 0, 1, 1),
               color: Color.Red * 0.5f
            );

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(TextureName.bullet_box),
                destinationRectangle: new Rectangle(hudFrame.AvatarX + 24, hudFrame.AvatarY - 64, 32, 32),
                color: Color.White
             );

            _spriteBatch.DrawString(
                DrawingHelper.GetFont, $"x {hudFrame.Ammo}", new Vector2(hudFrame.AvatarX + 64, hudFrame.AvatarY - 56), Color.White
            );


            _spriteBatch.End();
        }

        public static void DrawCollisionBoxes(IEnumerable<Rectangle> rectangles)
        {
            _spriteBatch.Begin();

            var scaleConstant = DrawingHelper.ScaleConstant;

            foreach (var rectangle in rectangles)
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
