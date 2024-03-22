
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
        public static SpriteFont TinyFont { get; private set; }
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
                return 1536 * 48 / SceneSize.X; //3072
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
            TinyFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "tiny_font"));
            MediumFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "medium_font"));

            GameFont.Spacing = 4f;
            MediumFont.Spacing = 4f;
            SmallFont.Spacing = 4f;
            TinyFont.Spacing = 0f;
        }

        public static Rectangle GetDestinationRectangle(Point point, Point? size = null)
        {
            bool levelPreview = GameStateManager.State == State.Menu && MenuManager.LobbyFrame != null;

            var xOffset = levelPreview ? 480 : 0;
            var yOffset = levelPreview ? 160 + (TileSize == 6 ? 140 : 0) : 0;

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

        public static Rectangle GetTooltipRec(int index)
        {
            var sizeX = 896;
            var sizeY = 200;
            var offset = 16;

            return index switch
            {
                1 => new Rectangle(1920 - sizeX - offset, offset, sizeX, sizeY),
                2 => new Rectangle(offset, 1080 - sizeY - offset, sizeX, sizeY),
                3 => new Rectangle(1920 - sizeX - offset, 1080 - sizeY - offset, sizeX, sizeY),
                _ => new Rectangle(offset, offset, sizeX, sizeY)
            };
        }

        private static readonly int TeamScoreRecWidth = 96;
        private static readonly int TeamScoreRecHeight = 48;

        public static Rectangle GetTeamScoreRec(int index, bool background = false)
        {
            return new Rectangle(
                16 + (index * TeamScoreRecWidth) + (index * (352 - TeamScoreRecWidth)) - (background ? 2 : 0),
                16 - (background ? 2 : 0),
                background ? (TeamScoreRecWidth + 4) : TeamScoreRecWidth,
                background ? (TeamScoreRecHeight + 4) : TeamScoreRecHeight
            );
        }
    }
}
