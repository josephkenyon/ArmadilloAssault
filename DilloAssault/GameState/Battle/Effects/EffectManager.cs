using DilloAssault.Configuration;
using DilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Effects
{
    public static class EffectManager
    {
        public static List<Effect> Effects { get; private set; }

        public static void Initialize()
        {
            Effects = [];
        }

        public static void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null)
        {
            var effectConfiguration = ConfigurationManager.GetEffectConfiguration(effectType.ToString());

            var effectPosition = new Vector2(
                position.X - effectConfiguration.Size.X / 2,
                position.Y - effectConfiguration.Size.Y / 2
            );

            if (direction != null && weaponAngle != null)
            {
                effectPosition = new Vector2(
                    effectPosition.X + (float)(effectConfiguration.SpriteSize.X / 8 * Math.Cos((double)weaponAngle)),
                    effectPosition.Y + (float)(effectConfiguration.SpriteSize.Y / 8 * Math.Sin((double)weaponAngle))
                );
            }

            var effect = new Effect
            {
                Position = effectPosition,
                Direction = direction,
                Type = effectType
            };

            Effects.Add(effect);
        }

        public static void UpdateEffects()
        {
            Effects.RemoveAll(effect => effect.FrameCounter == ConfigurationManager.GetEffectConfiguration(effect.Type.ToString()).FrameLife);

            foreach (var effect in Effects)
            {
                effect.FrameCounter++;
            }
        }
    }
}
