﻿using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;
using ArmadilloAssault.GameState.Battle.Camera;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableEffect(EffectType type, Vector2 position, Direction? direction, int frame) : IDrawableObject
    {
        private readonly EffectJson Configuration = ConfigurationManager.GetEffectConfiguration(type);
        public TextureName Texture => Configuration.TextureName;

        public Rectangle GetDestinationRectangle() => new(position.ToPoint() - CameraManager.Offset, Configuration.Size.ToPoint());
        public Rectangle? GetSourceRectangle() => new Rectangle(
            (frame % Configuration.SpriteRowLength) * Configuration.SpriteSize.X,
            (frame / Configuration.SpriteRowLength) * Configuration.SpriteSize.Y,
            Configuration.SpriteSize.X,
            Configuration.SpriteSize.Y
        );

        public Direction GetDirection() => direction == Direction.Left ? Direction.Left : Direction.Right;
    }
}
