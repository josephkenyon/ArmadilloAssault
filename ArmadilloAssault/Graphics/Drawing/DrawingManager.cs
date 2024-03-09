using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing.Textures;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using ArmadilloAssault.GameState.Menu.Assets;
using ArmadilloAssault.GameState.Menu;
using System.Linq;
using ArmadilloAssault.GameState.Battle.Camera;

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
                texture: TextureManager.GetTexture(drawableObject.Texture),
                destinationRectangle: drawableObject.GetDestinationRectangle(),
                sourceRectangle: drawableObject.GetSourceRectangle(),
                color: DrawingHelper.GetColor(drawableObject.Color, drawableObject.Z) * drawableObject.Opacity,
                rotation: drawableObject.GetRotation(),
                origin: drawableObject.GetOrigin(),
                effects: drawableObject.GetDirection() == Direction.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: drawableObject.LayerDepth
            );
        }

        public static void DrawTexture(TextureName textureName, Rectangle destinationRectangle, float opacity = 1f, Rectangle? sourceRectangle = null)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
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

        public static void DrawStrings(IEnumerable<string> texts, IEnumerable<Vector2> positions)
        {
            _spriteBatch.Begin();

            var index = 0;
            var font = DrawingHelper.GetFont;
            foreach (var text in texts)
            {
                var measureString = font.MeasureString(text);
                _spriteBatch.DrawString(DrawingHelper.GetFont, text, positions.ElementAt(index++) - (measureString / 2), Color.White);
            }

            _spriteBatch.End();
        }

        private static Color GetPlayerColor(int index)
        {
            if (index == 1)
            {
                return new Color(255, 80, 80);
            }
            else if (index == 2)
            {
                return new Color(80, 180, 80);
            }
            else if (index == 3)
            {
                return new Color(180, 120, 80);
            }

            return new Color(80, 80, 255);
        }

        public static void DrawLobbyPlayerBackgrounds(IEnumerable<Rectangle> lobbyPlayerRectangles, IEnumerable<int> lobbyPlayerIds)
        {
            _spriteBatch.Begin();

            var index = 0;
            foreach (var rectangle in lobbyPlayerRectangles)
            {
                var id = lobbyPlayerIds.ElementAt(index);

                _spriteBatch.Draw(
                     texture: TextureManager.GetTexture(TextureName.white_pixel),
                     destinationRectangle: new Rectangle(rectangle.X - 2, rectangle.Y - 2, rectangle.Width + 4, rectangle.Height + 4),
                     sourceRectangle: new Rectangle(0, 0, 1, 1),
                     color: Color.White
                 );

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: rectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: GetPlayerColor(id)
                );

                var text = $"P{id + 1}";
                var font = DrawingHelper.GetFont;
                var size = font.MeasureString(text);

                var textPosition = new Point(rectangle.Center.X - (int)(size.X / 2), rectangle.Bottom - 32 - (int)(size.Y / 2));

                _spriteBatch.DrawString(
                    font, text, textPosition.ToVector2(), Color.White
                );

                index++;
            }

            _spriteBatch.End();
        }

        public static void DrawMenuButtons(IEnumerable<Button> buttons)
        {
            _spriteBatch.Begin();

            foreach (var button in buttons)
            {
                var destinationRectangle = button.GetRectangle();

                _spriteBatch.Draw(
                     texture: TextureManager.GetTexture(TextureName.white_pixel),
                     destinationRectangle: new Rectangle(destinationRectangle.X - 2, destinationRectangle.Y - 2, destinationRectangle.Width + 4, destinationRectangle.Height + 4),
                     sourceRectangle: new Rectangle(0, 0, 1, 1),
                     color: Color.White
                 );

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: (button.Selected ? MenuManager.ForegroundColor : MenuManager.BackgroundColor)
                );

                if (button.TextureName != TextureName.nothing)
                {
                    _spriteBatch.Draw(
                        texture: TextureManager.GetTexture(button.TextureName),
                        destinationRectangle: destinationRectangle,
                        color: Color.White
                    );
                }
                
                if (button.Text != null)
                {
                    var font = DrawingHelper.MenuFont;
                    var size = font.MeasureString(button.Text);

                    var textPosition = destinationRectangle.Center - (size / 2).ToPoint();

                    _spriteBatch.DrawString(
                        font, button.Text, textPosition.ToVector2(), Color.White
                    );
                }

                if (!button.Enabled)
                {
                    _spriteBatch.Draw(
                        texture: TextureManager.GetTexture(TextureName.white_pixel),
                        destinationRectangle: destinationRectangle,
                        sourceRectangle: new Rectangle(0, 0, 1, 1),
                        color: Color.Black * 0.5f
                    );
                }
            }

            _spriteBatch.End();
        }

        public static void DrawHud(HudFrame hudFrame)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(TextureName.white_pixel),
                destinationRectangle: new Rectangle(hudFrame.AvatarX + 16 - CameraManager.CameraOffset.X, hudFrame.AvatarY - 24 - CameraManager.CameraOffset.Y, 96, 8),
                sourceRectangle: new Rectangle(0, 0, 1, 1),
                color: Color.Black * 0.5f
            );

            _spriteBatch.Draw(
               texture: TextureManager.GetTexture(TextureName.white_pixel),
               destinationRectangle: new Rectangle(
                   hudFrame.AvatarX + 16 - CameraManager.CameraOffset.X,
                   hudFrame.AvatarY - 24 - CameraManager.CameraOffset.Y,
                   Math.Clamp((int)(96f * (hudFrame.Health / 100f)), 2, 100), 8
                ),
               sourceRectangle: new Rectangle(0, 0, 1, 1),
               color: Color.Red * 0.5f
            );

            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(TextureName.bullet_box),
                destinationRectangle: new Rectangle(
                    hudFrame.AvatarX + 24 - CameraManager.CameraOffset.X,
                    hudFrame.AvatarY - 64 - CameraManager.CameraOffset.Y,
                    32, 32
                ),
                color: Color.White
             );

            _spriteBatch.DrawString(
                DrawingHelper.GetFont, $"x {hudFrame.Ammo}",
                new Vector2(
                    hudFrame.AvatarX + 64 - CameraManager.CameraOffset.X,
                    hudFrame.AvatarY - 56 - CameraManager.CameraOffset.Y
                ),
                Color.White
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
