
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ArmadilloAssault.Graphics.Drawing
{
    public static class DrawingHelper
    {
        private static SpriteFont EditorFont { get; set; }
        public static SpriteFont MenuFont { get; private set; }
        private static SpriteFont GameFont { get; set; }

        public static readonly int FullTileSize = 48;

        public static Color GetColor(Color color, int z)
        {
            if (z >= 0)
            {
                return color;
            }
            else
            {
                var constant = (1f + (z / 5f));
                return color * constant;
            }
        }

        public static Point SceneSize { get; set; }
        private static int SolveLinearFunction(double x)
        {
            double slope = -1.0 / 96.0; // Slope
            double intercept = 60.0; // Y-intercept
            double result = slope * x + intercept;
            return (int)Math.Round(result, MidpointRounding.AwayFromZero);
        }
        public static int TileSize => GameStateManager.State == State.Editor ? SolveLinearFunction(SceneSize.X) : 48;

        public static float ScaleConstant => (float) TileSize / FullTileSize;
        public static float ReverseScaleConstant => (float)FullTileSize / TileSize;

        public static SpriteFont GetFont => GameStateManager.State == State.Editor ? EditorFont : GameStateManager.State == State.Menu ? MenuFont : GameFont;
          
        public static void LoadContent(ContentManager contentManager)
        {
            EditorFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "editor_font"));
            GameFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "game_font"));
            MenuFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "menu_font"));
        }

        public static Rectangle GetDestinationRectangle(Point point, Point? size = null)
        {
            var newSize = size ?? new Point(1, 1);
            return new Rectangle(point.X * TileSize - CameraManager.CameraOffset.X, point.Y * TileSize - CameraManager.CameraOffset.Y, newSize.X * TileSize, newSize.Y * TileSize);
        }

        public static Rectangle GetDestinationRectangle(Vector2 point, Point? size = null)
        {
            var newSize = size ?? new Point(1, 1);
            return new Rectangle((int)(point.X * TileSize), (int)(point.Y * TileSize), newSize.X * TileSize, newSize.Y * TileSize);
        }

        public static Rectangle GetSourceRectangle(Point point, Point? size = null)
        {
            var newSize = size ?? new Point(1, 1);
            return new Rectangle(point.X * FullTileSize, point.Y * FullTileSize, newSize.X * FullTileSize, newSize.Y * FullTileSize);
        }
    }
}
