
using ArmadilloAssault.Assets;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Menus;
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
        public static SpriteFont MediumFont { get; private set; }
        public static SpriteFont SmallFont { get; private set; }
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
                var constant = (1f + ((z + 1) / 5f));
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

        public static Color GetTeamColor(int index)
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
                return new Color(200, 200, 60);
            }
            else if (index == 4)
            {
                return new Color(70, 200, 200);
            }
            else if (index == 5)
            {
                return new Color(200, 70, 190);
            }

            return new Color(80, 80, 255);
        }

        public static int TileSize => GetTileSize();

        private static int GetTileSize()
        {
            if (GameStateManager.State == State.Editor)
            {
                return SolveLinearFunction(SceneSize.X);
            }
            else if (GameStateManager.State == State.Menu && MenuManager.LobbyFrame != null)
            {
                return MenuManager.LobbyFrame.TileSize;
            }

            return 48;
        }

        public static float ScaleConstant => (float) TileSize / FullTileSize;
        public static float ReverseScaleConstant => (float)FullTileSize / TileSize;

        public static SpriteFont GetFont => GameStateManager.State == State.Editor ? EditorFont : GameStateManager.State == State.Menu ? MediumFont : GameFont;
          
        public static void LoadContent(ContentManager contentManager)
        {
            EditorFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "editor_font"));
            GameFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "game_font"));
            SmallFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "small_font"));
            MediumFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "medium_font"));

            GameFont.Spacing = 4f;
            MediumFont.Spacing = 4f;
            SmallFont.Spacing = 4f;
        }

        public static Rectangle GetDestinationRectangle(Point point, Point? size = null)
        {
            bool levelPreview = GameStateManager.State == State.Menu && MenuManager.LobbyFrame != null;

            var xOffset = levelPreview ? 480 : 0;
            var yOffset = levelPreview ? 160 : 0;

            var newSize = size ?? new Point(1, 1);

            return new Rectangle(point.X * TileSize - CameraManager.Offset.X + xOffset, point.Y * TileSize - CameraManager.Offset.Y + yOffset, newSize.X * TileSize, newSize.Y * TileSize);
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
