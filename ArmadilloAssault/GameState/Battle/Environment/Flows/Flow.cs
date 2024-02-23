using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Environment.Flows
{
    public class Flow(FlowJson flowJson, int x) : IDrawableObject
    {
        public TextureName Texture => flowJson.TextureName;
        public Point Size { get; set; } = new Point(flowJson.Size.X * 2, flowJson.Size.Y * 2);
        public int Y { get; set; } = flowJson.Y;
        public int X { get; set; } = x;
        public float Opacity => 0.75f;

        public Rectangle GetDestinationRectangle() => new(X, Y, Size.X, Size.Y);
    }
}
