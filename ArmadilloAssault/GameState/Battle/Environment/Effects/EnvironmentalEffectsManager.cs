using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Scenes;
using System.Collections.Generic;
using System;
using ArmadilloAssault.Configuration.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Environment.Effects
{
    public class EnvironmentalEffectsManager {
        public List<EnvironmentalEffect> Effects { get; private set; }
        private int SpawnRate { get; set; } = -1;
        private RangeJson YRange { get; set; }
        private EffectType EffectType { get; set; }
        private Random Random { get; set; }
        private int FrameCounter { get; set; }

        public EnvironmentalEffectsManager(EnvironmentalEffectJson environmentalEffectJson)
        {
            Effects = [];
            SpawnRate = -1;

            if (environmentalEffectJson != null)
            {
                SpawnRate = environmentalEffectJson.SpawnRate;
                EffectType = environmentalEffectJson.EffectType;
                YRange = environmentalEffectJson.YRange;

                Random = new Random();

                for (int i = 0; i < environmentalEffectJson.StartingCount; i++)
                {
                    CreateNewEffect();
                }
            }
        }

        public void CreateNewEffect()
        {
            var effectConfiguration = ConfigurationManager.GetEffectConfiguration(EffectType);

            var x = Random.Next(-effectConfiguration.Size.X, 1920);
            var y = Random.Next(YRange.Start - effectConfiguration.Size.Y, YRange.End - effectConfiguration.Size.Y);
            var effectPosition = new Point(x, y);

            Effects.Add(new EnvironmentalEffect
            {
                EffectType = EffectType,
                Texture = effectConfiguration.TextureName,
                DestinationRectangle = new Rectangle(effectPosition, effectConfiguration.Size.ToPoint())
            });
        }

        public void UpdateEffects()
        {
            if (SpawnRate != -1)
            {
                var effectConfiguration = ConfigurationManager.GetEffectConfiguration(EffectType);
                Effects.RemoveAll(effect => effect.FrameCounter == effectConfiguration.FrameLife);

                if (FrameCounter == SpawnRate)
                {
                    CreateNewEffect();
                    FrameCounter = 0;
                }

                foreach (var effect in Effects)
                {
                    effect.FrameSkip++;

                    if (effect.FrameSkip == effectConfiguration.FrameSkip)
                    {
                        effect.FrameSkip = 0;
                        effect.FrameCounter++;
                    }
                }

                FrameCounter++;
            }
        }
    }
}