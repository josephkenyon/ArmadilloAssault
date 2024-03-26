using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmadilloAssault.GameState.Battle.Effects
{
    public class EffectManager(IEffectManagerListener listener)
    {
        public List<Effect> Effects { get; private set; } = [];

        public void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null, bool fromUpdate = false)
        {
            var effectConfiguration = ConfigurationManager.GetEffectConfiguration(effectType);

            var effectPosition = new Vector2(
                position.X - (fromUpdate ? 0 : (effectConfiguration.Size.X / 2)),
                position.Y - (fromUpdate ? 0 : (effectConfiguration.Size.Y / 2))
            );

            if (direction != null && weaponAngle != null)
            {
                effectPosition = new Vector2(
                    effectPosition.X + (float)(effectConfiguration.SpriteSize.X / 8 * Math.Cos((double)weaponAngle)),
                    effectPosition.Y + (float)(effectConfiguration.SpriteSize.Y / 8 * Math.Sin((double)weaponAngle))
                );
            }

            var effect = new Effect(effectType, effectPosition, direction);

            Effects.Add(effect);

            if (!fromUpdate)
            {
                listener.EffectCreated(effect);
            }
        }

        public void UpdateEffects()
        {
            Effects.RemoveAll(effect => effect.FrameCounter == ConfigurationManager.GetEffectConfiguration(effect.Type).FrameLife);

            foreach (var effect in Effects)
            {
                effect.FrameCounter++;
            }
        }

        public ICollection<DrawableEffect> GetDrawableEffects()
        {
            var drawableEffects = new List<DrawableEffect>();

            var index = 0;
            foreach (var effect in Effects)
            {
                try
                {
                    var drawableEffect = new DrawableEffect(
                        effect.Type,
                        effect.Position,
                        effect.Direction,
                        effect.FrameCounter
                    );

                    drawableEffects.Add(drawableEffect);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }

                index++;
            }

            return drawableEffects;
        }

        internal void DeleteEffects(List<int> deletedIds)
        {
            if (deletedIds != null)
            {
                Effects.RemoveAll(effect => deletedIds.Contains(effect.id));
            }
        }
    }
}
