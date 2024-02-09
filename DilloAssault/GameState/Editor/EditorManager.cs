using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.Configuration.Textures;
using DilloAssault.Controls;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text.Json;

namespace DilloAssault.GameState.Editor
{
    public static class EditorManager
    {
        private static readonly Point SceneSize = new(40, 23);
        public static Scene Scene { get; set; }

        public static int Z { get; set; }
        public static Point SpriteSelectionIndex { get; set; }
        public static TextureName SelectedTextureName { get; set; }

        public static void Initialize(Scene scene)
        {
            Scene = scene;
            Z = -1;
            SelectedTextureName = TextureName.test_tileset;
            SpriteSelectionIndex = new Point(0, 0);
        }

        public static void Update()
        {
            const int playerIndex = 0;

            var aimPosition = ControlsManager.GetAimPosition(playerIndex);

            var xTileIndex = (int)(aimPosition.X / DrawingHelper.TileSize);
            var yTileIndex = (int)(aimPosition.Y / DrawingHelper.TileSize);

            if (xTileIndex < 0 || yTileIndex < 0)
            {
                return;
            }

            if (ControlsManager.IsControlDown(playerIndex, Control.Confirm))
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
                var json = JsonSerializer.Serialize(ConfigurationHelper.GetSceneJson(Scene));
                File.WriteAllText(Path.Combine(ConfigurationHelper.GetConfigurationPath("Scenes"), "jungle_scene.json"), json);
            }
        }

        public static void Draw()
        {
            var scaleConstant = DrawingHelper.ScaleConstant;
            var scaledTileSize = DrawingHelper.TileSize;

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, (int)(1920 * DrawingHelper.ScaleConstant), (int)(1080 * DrawingHelper.ScaleConstant)));

            Scene.TileLists.ForEach(list => DrawingManager.DrawCollection([.. list.Tiles]));

            DrawingManager.DrawTexture(TextureName.test_tileset, new Vector2((SceneSize.X + 1) * scaledTileSize, 1 * scaledTileSize));

            DrawingManager.DrawString($"Z: {Z}", new Point(SceneSize.X, 0), DrawingHelper.GetFont);

            if (Z == 0)
            {
                DrawingManager.DrawCollisionBoxes(Scene.CollisionBoxes);
            }
        }
    }
}
