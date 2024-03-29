﻿using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text.Json;

namespace ArmadilloAssault.GameState.Editor
{
    public static class EditorManager
    {
        private static Point SceneSize = new(40, 23);
        public static Scene Scene { get; set; }

        public static int Z { get; set; }
        public static Point SpriteSelectionIndex { get; set; }
        public static TextureName SelectedTextureName { get; set; }

        public static bool ClickFrameDebounce { get; set; }

        public static void Initialize()
        {
            Mouse.SetCursor(MouseCursor.Arrow);
            GraphicsManager.AllowResizing();

            Scene = new Scene(ConfigurationManager.GetSceneConfiguration("editor"));

            SceneSize = new Point(
                (int)Math.Round(Scene.Size.X / 48d, MidpointRounding.AwayFromZero),
                (int)Math.Round(Scene.Size.Y / 48d, MidpointRounding.AwayFromZero)
            );

            DrawingHelper.SceneSize = Scene.Size;

            Z = 1;

            SelectedTextureName = Scene.TilesetTexture;
            SpriteSelectionIndex = new Point(0, 0);

            ClickFrameDebounce = false;
        }

        public static void Update()
        {
            const int playerIndex = 0;

            var aimPosition = ControlsManager.GetMousePosition(playerIndex);

            var xTileIndex = (int)(aimPosition.X / DrawingHelper.TileSize);
            var yTileIndex = (int)(aimPosition.Y / DrawingHelper.TileSize);

            if (xTileIndex < 0 || yTileIndex < 0)
            {
                return;
            }

            if (ControlsManager.IsControlDown(playerIndex, Control.Confirm) && ClickFrameDebounce)
            {
                if (xTileIndex < SceneSize.X && yTileIndex < SceneSize.Y)
                {
                    if (Z == 0)
                    {
                        var scaleConstant = DrawingHelper.ReverseScaleConstant;
                        if (ControlsManager.IsControlDownStart(playerIndex, Control.Confirm))
                        {
                            Scene.CollisionBoxes.Add(new Rectangle((int)(aimPosition.X * scaleConstant), (int)(aimPosition.Y * scaleConstant), 0, 0));
                        }
                        else
                        {
                            Scene.UpdateCollisionBox(aimPosition * scaleConstant);
                        }
                    }
                    else
                    {
                        Scene.UpdateTile(Z, new Point(xTileIndex, yTileIndex), SpriteSelectionIndex, SelectedTextureName);
                    }
                }
                else if (xTileIndex > SceneSize.X)
                {
                    var spriteSelectionOrigin = new Point(SceneSize.X + 1, 1).ToVector2() * DrawingHelper.TileSize;

                    SpriteSelectionIndex = new Point(
                        (int)(aimPosition.X - spriteSelectionOrigin.X) / DrawingHelper.FullTileSize,
                        (int)(aimPosition.Y - spriteSelectionOrigin.Y) / DrawingHelper.FullTileSize
                    );
                }
            }
            else if (ControlsManager.IsControlDown(playerIndex, Control.Fire_Secondary))
            {
                if (xTileIndex < SceneSize.X && yTileIndex < SceneSize.Y)
                {
                    if (Z == 0)
                    {
                        Scene.DeleteCollisionBox(aimPosition * DrawingHelper.ReverseScaleConstant);
                    }
                    else
                    {
                        Scene.DeleteTile(Z, new Point(xTileIndex, yTileIndex));
                    }
                }
            }

            if (ControlsManager.IsControlPressed(playerIndex, Control.Down))
            {
                Z--;
            }
            else if (ControlsManager.IsControlPressed(playerIndex, Control.Up))
            {
                Z++;
            }

            if (ControlsManager.IsControlPressed(playerIndex, Control.Start))
            {
                if (Scene.CollisionBoxes.Count > 0)
                {
                    var json = JsonSerializer.Serialize(ConfigurationHelper.GetSceneJson(Scene));
                    File.WriteAllText(Path.Combine(ConfigurationHelper.GetConfigurationPath("Scenes"), "editor.json"), json);
                }
             
                ConfigurationManager.LoadScenes();

                GameStateManager.PushNewState(State.Menu);

                Mouse.SetCursor(MouseCursor.FromTexture2D(TextureManager.GetTexture(TextureName.cursor), 0, 0));
            }

            if (!ControlsManager.IsControlDown(playerIndex, Control.Confirm))
            {
                ClickFrameDebounce = true;
            }
        }

        public static void Draw()
        {
            var scaleConstant = DrawingHelper.ScaleConstant;
            var scaledTileSize = DrawingHelper.TileSize;

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, (int)(Scene.Size.X * DrawingHelper.ScaleConstant), (int)(Scene.Size.Y * DrawingHelper.ScaleConstant)));

            Scene.TileLists.ForEach(list => DrawingManager.DrawCollection([.. list.Tiles]));

            DrawingManager.DrawTexture(Scene.TilesetTexture, new Vector2((SceneSize.X + 1) * scaledTileSize, 1 * scaledTileSize));

            DrawingManager.DrawEditorString($"Z: {Z}", new Point(SceneSize.X, 0), DrawingHelper.GetFont);

            var aimPosition = ControlsManager.GetMousePosition(0);

            var xTileIndex = (int)(aimPosition.X / DrawingHelper.TileSize);
            var yTileIndex = (int)(aimPosition.Y / DrawingHelper.TileSize);

            DrawingManager.DrawEditorString($"X: {xTileIndex}, Y: {yTileIndex}", new Point(SceneSize.X + 4, 0), DrawingHelper.GetFont);

            if (Z == 0)
            {
                DrawingManager.DrawRectangles(Scene.CollisionBoxes);
            }
        }
    }
}
