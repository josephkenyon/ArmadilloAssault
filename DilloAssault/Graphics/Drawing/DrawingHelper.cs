﻿
using DilloAssault.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace DilloAssault.Graphics.Drawing
{
    public static class DrawingHelper
    {
        private static SpriteFont EditorFont { get; set; }

        public static readonly int FullTileSize = 48;
        private static readonly int SpriteTileSize = 48;

        public static Color GetColor(int z)
        {
            if (z >= 0)
            {
                return Color.White;
            }
            else
            {
                var constant = (1f + (z / 5f));
                return Color.White * constant;
            }
        }
        public static int TileSize => GameStateManager.State == State.Editor ? 32 : 48;
        public static SpriteFont GetFont => GameStateManager.State == State.Editor ? EditorFont : EditorFont;

        public static void LoadContent(ContentManager contentManager)
        {
            EditorFont = contentManager.Load<SpriteFont>(Path.Combine("Graphics", "Fonts", "File"));
        }

        public static Rectangle GetDestinationRectangle(Point point, Point? size = null)
        {
            var newSize = size ?? new Point(1, 1);
            return new Rectangle(point.X * TileSize, point.Y * TileSize, newSize.X * TileSize, newSize.Y * TileSize);
        }

        public static Rectangle GetSourceRectangle(Point point, Point? size = null)
        {
            var newSize = size ?? new Point(1, 1);
            return new Rectangle(point.X * SpriteTileSize, point.Y * SpriteTileSize, newSize.X * SpriteTileSize, newSize.Y * SpriteTileSize);
        }
    }
}
