using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Environment.Effects
{
    public class EnvironmentalEffect : IDrawableObject
    {
        private static Random Random = new();
        public EffectType EffectType { get; set; }
        public TextureName Texture { get; set; }
        public Rectangle DestinationRectangle { get; set; }
        public int FrameCounter { get; set; } = 0;
        public int FrameSkip { get; set; } = 0;
        public float Scaler { get; set; } = (float)Random.NextDouble();

        public TextureName TextureName => Texture;
        public Rectangle GetDestinationRectangle() => CameraManager.GetEnvironmentalEffectDestinationRectangle(DestinationRectangle);
        public float Opacity => (float)Math.Sin(2 * Math.PI * FrameCounter / 120) * 0.66f;
        private EffectJson Configuration => ConfigurationManager.GetEffectConfiguration(EffectType);
        public Rectangle? GetSourceRectangle() => new Rectangle(
            (FrameCounter % Configuration.SpriteRowLength) * Configuration.SpriteSize.X,
            (FrameCounter / Configuration.SpriteRowLength) * Configuration.SpriteSize.Y,
            Configuration.SpriteSize.X,
            Configuration.SpriteSize.Y
        );
    }
}
