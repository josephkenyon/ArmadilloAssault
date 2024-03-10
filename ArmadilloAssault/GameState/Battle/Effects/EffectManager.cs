using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmadilloAssault.GameState.Battle.Effects
{
    public class EffectManager
    {
        public List<Effect> Effects { get; private set; } = [];

        public void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null)
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

        public void UpdateEffects()
        {
            Effects.RemoveAll(effect => effect.FrameCounter == ConfigurationManager.GetEffectConfiguration(effect.Type).FrameLife);

            foreach (var effect in Effects)
            {
                effect.FrameCounter++;
            }
        }

        public EffectFrame GetEffectFrame()
        {
            var effectFrame = new EffectFrame();

            foreach (var effect in Effects)
            {
                effectFrame.Types.Add(effect.Type);
                effectFrame.PositionXs.Add(effect.Position.X);
                effectFrame.PositionYs.Add(effect.Position.Y);
                effectFrame.Directions.Add(effect.Direction);
                effectFrame.Frames.Add(effect.FrameCounter);
            }

            return effectFrame;
        }

        public static ICollection<DrawableEffect> GetDrawableEffects(EffectFrame effectFrame)
        {
            var drawableEffects = new List<DrawableEffect>();

            var index = 0;
            foreach (var type in effectFrame.Types)
            {
                try
                {
                    var drawableEffect = new DrawableEffect(
                        type,
                        new Vector2(effectFrame.PositionXs[index], effectFrame.PositionYs[index]),
                        effectFrame.Directions[index],
                        effectFrame.Frames[index]
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
    }
}
