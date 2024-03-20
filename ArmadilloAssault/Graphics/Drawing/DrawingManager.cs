using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing.Textures;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using ArmadilloAssault.GameState.Menus.Assets;
using ArmadilloAssault.GameState.Menus;
using System.Linq;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle.Mode;

namespace ArmadilloAssault.Graphics.Drawing
{
    static class DrawingManager
    {
        private static SpriteBatch _spriteBatch;

        public static void LoadContent(GraphicsDevice graphicsDevice) {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public static void Begin()
        {
            _spriteBatch.Begin();
        }

        public static void End()
        {
            _spriteBatch.End();
        }

        public static void DrawCollection(IEnumerable<IDrawableObject> drawableObjects)
        {
            foreach (var drawableObject in drawableObjects)
            {
                DrawObject(drawableObject);
            }
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

        public static void DrawTexture(TextureName textureName, Rectangle destinationRectangle, float opacity = 1f, Rectangle? sourceRectangle = null, Color? color = null)
        {
            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
                rotation: 0f,
                effects: SpriteEffects.None,
                origin: Vector2.Zero,
                color: (color ?? Color.White) * opacity,
                layerDepth: 1f
            );
        }

        public static void DrawTexture(TextureName textureName, Vector2 position)
        {
            _spriteBatch.Draw(
                texture: TextureManager.GetTexture(textureName),
                position: position,
                color: Color.White
            );
        }

        public static void DrawEditorString(string text, Point position, SpriteFont spriteFont)
        {
            _spriteBatch.DrawString(spriteFont, text, position.ToVector2() * DrawingHelper.TileSize, Color.White);
        }

        public static void DrawString(string text, Vector2 position, SpriteFont spriteFont = null)
        {
            var font = spriteFont ?? DrawingHelper.GetFont;
            var measureString = font.MeasureString(text);
            _spriteBatch.DrawString(font, text, position - (measureString / 2), Color.White);
        }


        public static void DrawStrings(IEnumerable<string> texts, IEnumerable<Vector2> positions, SpriteFont spriteFont = null)
        {
            var index = 0;
            var font = spriteFont ?? DrawingHelper.GetFont;
            foreach (var text in texts)
            {
                var measureString = font.MeasureString(text);
                _spriteBatch.DrawString(font, text, positions.ElementAt(index++) - (measureString / 2), Color.White);
            }
        }      

        public static void DrawLobbyPlayerBackgrounds(IEnumerable<Rectangle> lobbyPlayerRectangles, IEnumerable<int> lobbyTeamIds, IEnumerable<int> lobbyPlayerIds)
        {
            var index = 0;
            foreach (var rectangle in lobbyPlayerRectangles)
            {
                var playerId = lobbyPlayerIds.ElementAt(index);
                var teamId = lobbyTeamIds.ElementAt(index);

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
                    color: DrawingHelper.GetTeamColor(teamId)
                );

                index++;
            }
        }

        public static void DrawLobbyPlayerNames(IEnumerable<Rectangle> lobbyPlayerRectangles, IEnumerable<int> lobbyTeamIds, IEnumerable<int> lobbyPlayerIds)
        {
            var index = 0;
            foreach (var rectangle in lobbyPlayerRectangles)
            {
                var playerId = lobbyPlayerIds.ElementAt(index);
                var teamId = lobbyTeamIds.ElementAt(index);

                var text = $"P{playerId + 1}";
                var font = DrawingHelper.GetFont;
                var size = font.MeasureString(text);

                var textPosition = new Point(rectangle.Center.X - (int)(size.X / 2), rectangle.Bottom - 32 - (int)(size.Y / 2));

                _spriteBatch.DrawString(
                    font, text, textPosition.ToVector2(), Color.White
                );

                index++;
            }
        }

        public static void DrawLobbyPlayerCrowns(IEnumerable<Rectangle> lobbyPlayerRectangles)
        {
            var index = 0;
            foreach (var rectangle in lobbyPlayerRectangles)
            {
                var texture = TextureManager.GetTexture(TextureName.crown);
                var crownPosition = new Vector2(rectangle.Center.X - (texture.Width / 2), rectangle.Center.Y - (texture.Height / 2));

                _spriteBatch.Draw(
                     texture: texture,
                     position: crownPosition,
                     color: Color.White
                 );

                index++;
            }
        }

        public static void DrawMenuButtons(IEnumerable<Button> buttons)
        {
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
                    color: (button.Selected ? MenuManager.ForegroundColor : button.Unselectable ? MenuManager.DarkBackgroundColor : MenuManager.BackgroundColor)
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
                    var font = DrawingHelper.MediumFont;
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
        }

        public static void DrawHud(HudFrame hudFrame, int playerIndex)
        {
            for (int i = 0; i < hudFrame.PlayerIndices.Count; i++)
            {
                var avatarIndex = hudFrame.PlayerIndices[i];

                if ((!hudFrame.Visibles[i] && playerIndex != avatarIndex) || hudFrame.Deads[i]) { continue; }

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: new Rectangle(hudFrame.AvatarXs[i] + 16 - CameraManager.Offset.X, hudFrame.AvatarYs[i] - 24 - CameraManager.Offset.Y, 96, 8),
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: Color.Black * 0.5f
                );

                _spriteBatch.Draw(
                   texture: TextureManager.GetTexture(TextureName.white_pixel),
                   destinationRectangle: new Rectangle(
                       hudFrame.AvatarXs[i] + 16 - CameraManager.Offset.X,
                       hudFrame.AvatarYs[i] - 24 - CameraManager.Offset.Y,
                       Math.Clamp((int)(96f * (hudFrame.Healths[i] / 100f)), 2, 100), 8
                    ),
                   sourceRectangle: new Rectangle(0, 0, 1, 1),
                   color: Color.Red * 0.5f
                );

                if (avatarIndex == playerIndex)
                {
                    _spriteBatch.Draw(
                        texture: TextureManager.GetTexture(TextureName.bullet_box),
                        destinationRectangle: new Rectangle(
                            hudFrame.AvatarXs[i] + 24 - CameraManager.Offset.X,
                            hudFrame.AvatarYs[i] - 64 - CameraManager.Offset.Y,
                            32, 32
                        ),
                        color: Color.White
                    );

                    _spriteBatch.DrawString(
                        DrawingHelper.GetFont, $"x {hudFrame.Ammos[i]}",
                        new Vector2(
                            hudFrame.AvatarXs[i] + 64 - CameraManager.Offset.X,
                            hudFrame.AvatarYs[i] - 56 - CameraManager.Offset.Y
                        ),
                        Color.White
                    );
                }
            }

            foreach (var teamIndex in hudFrame.TeamIndices)
            {
                _spriteBatch.Draw(
                   texture: TextureManager.GetTexture(TextureName.white_pixel),
                   destinationRectangle: DrawingHelper.GetTeamScoreRec(teamIndex, true),
                   sourceRectangle: new Rectangle(0, 0, 1, 1),
                   color: Color.White * 0.65f
                );

                var rec = DrawingHelper.GetTeamScoreRec(teamIndex, false);

                _spriteBatch.Draw(
                  texture: TextureManager.GetTexture(TextureName.white_pixel),
                  destinationRectangle: rec,
                  sourceRectangle: new Rectangle(0, 0, 1, 1),
                  color: DrawingHelper.GetTeamColor(teamIndex) * 0.65f
                );

                var modeValueIndex = hudFrame.TeamIndices.Distinct().ToList().IndexOf(teamIndex);

                var value = $"{hudFrame.ModeValues[modeValueIndex]}";
                var stringSize = DrawingHelper.GetFont.MeasureString(value);

                if (ModeType.Deathmatch == hudFrame.ModeType || ModeType.Regicide == hudFrame.ModeType)
                {
                    var texture = TextureManager.GetTexture(TextureName.skull);
                    _spriteBatch.Draw(
                      texture: texture,
                      position: new Vector2(rec.Center.X - 20 - (texture.Width / 2f), rec.Center.Y - (texture.Height / 2f)),
                      color: Color.White
                    );
                }

                _spriteBatch.DrawString(
                    DrawingHelper.GetFont, $"{hudFrame.ModeValues[modeValueIndex]}",
                    new Vector2(
                        rec.Center.X + (hudFrame.ModeType == ModeType.King_of_the_Hill ? 0 : 20) - (stringSize.X / 2),
                        rec.Center.Y - (stringSize.Y / 2)
                    ),
                    Color.White
                );
            }
        }

        public static void DrawRectangles(IEnumerable<Rectangle> rectangles, Color? color = null)
        {
            var scaleConstant = GameStateManager.State != State.Menu ? DrawingHelper.ScaleConstant : 1f;

            foreach (var rectangle in rectangles)
            {
                var destinationRectangle = new Rectangle((int)(rectangle.X * scaleConstant), (int)(rectangle.Y * scaleConstant), (int)(rectangle.Width * scaleConstant), (int)(rectangle.Height * scaleConstant));

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: (color ?? Color.Yellow) * (color != null ? 0.7f : 0.35f)
                );
            }
        }

        public static void DrawTooltip(List<string> texts, Point startingPoint)
        {
            var font = DrawingHelper.SmallFont;

            var position = new Point(startingPoint.X, startingPoint.Y);

            if (texts.Count > 0)
            {
                var measureString = font.MeasureString(texts.First());
                position = new Point(position.X, position.Y - (int)(texts.Count * measureString.Y / 2));
            }

            foreach (var text in texts)
            {
                var measureString = font.MeasureString(text);

                position = new Point(position.X, position.Y);

                _spriteBatch.DrawString(font, text, new Vector2(position.X - (measureString.X / 2), position.Y), Color.White);

                position = new Point(position.X, position.Y + (int)measureString.Y);

            }
        }

        public static void DrawBattleTooltips(List<List<string>> textsList)
        {
            var index = 0;
            foreach (var texts in textsList)
            {
                var destinationRectangle = DrawingHelper.GetTooltipRec(index);

                _spriteBatch.Draw(
                     texture: TextureManager.GetTexture(TextureName.white_pixel),
                     destinationRectangle: new Rectangle(destinationRectangle.X - 2, destinationRectangle.Y - 2, destinationRectangle.Width + 4, destinationRectangle.Height + 4),
                     sourceRectangle: new Rectangle(0, 0, 1, 1),
                     color: Color.White * 0.25f
                 );

                _spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: MenuManager.DarkBackgroundColor * 0.25f
                );

                DrawTooltip(texts, new Point(destinationRectangle.Center.X, destinationRectangle.Center.Y));

                index++;
            }
        }
    }
}
