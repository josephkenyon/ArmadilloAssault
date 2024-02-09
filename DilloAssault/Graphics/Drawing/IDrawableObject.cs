using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.Graphics.Drawing
{
    public interface IDrawableObject
    {
        int Z => 0;
        Vector2 Position { get; }
        Point SpriteLocation { get; }
        TextureName TextureName { get; }
        Rectangle GetDestinationRectangle();
        Rectangle? GetSourceRectangle() => null;
        Vector2 GetOrigin() => Vector2.Zero;
        float GetRotation() => 0f;
        float Opacity => 1f;
        float LayerDepth => 1f;
        Direction GetDirection() => Direction.Right;
    }
}
