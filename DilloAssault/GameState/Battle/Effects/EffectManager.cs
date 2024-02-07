using DilloAssault.Configuration;
using Microsoft.Xna.Framework;
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

        public static void CreateEffect(Vector2 position, EffectType effectType)
        {
            var effectConfiguration = ConfigurationManager.GetEffectConfiguration(effectType.ToString());

            var effect = new Effect
            {
                Position = new Vector2(
                    position.X - effectConfiguration.Size.X / 2,
                    position.Y - effectConfiguration.Size.Y / 2
                ),
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
