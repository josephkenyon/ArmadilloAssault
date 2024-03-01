﻿using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Crates;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableCrate(CrateType type, Vector2 position, bool grounded) : IDrawableObject
    {
        public Point Size => grounded ? new Point(64, 64) : new Point(128, 204);
        public TextureName Texture => grounded ? TextureName.crates : TextureName.crates_parachuting;
        public Rectangle? GetSourceRectangle() => new((int)type * Size.X, 0, Size.X, Size.Y);
        public Rectangle GetDestinationRectangle() => new((int)position.X - (!grounded ? 31 : 0), (int)position.Y - (!grounded ? 126 : 0), Size.X, Size.Y);
    }
}
