using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class LoadingSpinner(Point position) : IDrawableObject
    {
        public static readonly int Size = 128;

        private int FrameCounter { get; set; } = 0;
        private bool IncrementFrame { get; set; } = false;
        public Rectangle DestinationRectangle { get; set; } = new Rectangle(position, new Point(Size, Size));

        public TextureName Texture => TextureName.loading_spinner;
        public Rectangle GetDestinationRectangle() => DestinationRectangle;

        public Rectangle? GetSourceRectangle() => new Rectangle(
            (FrameCounter % 4) * Size,
            (FrameCounter / 4) * Size,
            Size,
            Size
        );

        public void Update()
        {
            IncrementFrame = !IncrementFrame;

            if (IncrementFrame)
            {
                FrameCounter++;
            }

            if (FrameCounter == 18)
            {
                FrameCounter = 0;
            }
        }
    }
}
