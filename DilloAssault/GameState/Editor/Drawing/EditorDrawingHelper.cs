using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Editor.Drawing
{
    public static class EditorDrawingHelper
    {
        public static void DrawCollisionBoxes(IEnumerable<Rectangle> rectangles)
        {
            var spriteBatch = DrawingManager.SpriteBatch;

            spriteBatch.Begin();

            var scaleConstant = DrawingHelper.ScaleConstant;

            foreach (var rectangle in rectangles)
            {
                var destinationRectangle = new Rectangle((int)(rectangle.X * scaleConstant), (int)(rectangle.Y * scaleConstant), (int)(rectangle.Width * scaleConstant), (int)(rectangle.Height * scaleConstant));

                spriteBatch.Draw(
                    texture: TextureManager.GetTexture(TextureName.white_pixel),
                    destinationRectangle: destinationRectangle,
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: Color.Yellow * 0.35f
                );
            }

            spriteBatch.End();
        }

        internal static void DrawCollisionBoxes(Func<Rectangle> getShellBox)
        {
            throw new NotImplementedException();
        }
    }
}
