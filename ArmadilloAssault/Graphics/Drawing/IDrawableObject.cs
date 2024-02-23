using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public interface IDrawableObject
    {
        int Z => 0;
        Point SpriteLocation => Point.Zero;
        TextureName Texture { get; }
        Rectangle GetDestinationRectangle();
        Rectangle? GetSourceRectangle() => null;
        Vector2 GetOrigin() => Vector2.Zero;
        float GetRotation() => 0f;
        float Opacity => 1f;
        float LayerDepth => 1f;
        Direction GetDirection() => Direction.Right;
    }
}
