using DilloAssault.Configuration;
using DilloAssault.Configuration.Effects;
using DilloAssault.Generics;
using DilloAssault.Web.Communication.Updates;
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
            var effectConfiguration = ConfigurationManager.GetEffectConfiguration(effectType);

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

            Effects.Add(new Effect(effectType, effectPosition, direction));
        }

        public static void UpdateEffects()
        {
            Effects.RemoveAll(effect => effect.FrameCounter == ConfigurationManager.GetEffectConfiguration(effect.Type).FrameLife);

            foreach (var effect in Effects)
            {
                effect.FrameCounter++;
            }
        }

        public static EffectsUpdate GetEffectsUpdate()
        {
            var effects = new EffectsUpdate();

            foreach (var effect in Effects)
            {
                effects.Types.Add(effect.Type);
                effects.Directions.Add(effect.GetDirection());
                effects.Xs.Add((int)effect.Position.X);
                effects.Ys.Add((int)effect.Position.Y);
                effects.Frames.Add(effect.FrameCounter);
            }

            return effects;
        }

        public static void UpdateEffects(EffectsUpdate effectsUpdate)
        {
            Effects = [];

            for (int i = 0; i < effectsUpdate.Types.Count; i++)
            {
                var effect = new Effect(effectsUpdate.Types[i], effectsUpdate.GetPosition(i), effectsUpdate.Directions[i])
                {
                    FrameCounter = effectsUpdate.Frames[i]
                };

                Effects.Add(effect);
            }
        }
    }
}
