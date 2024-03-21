using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Environment.Precipitation
{
    public class PrecipitationManager
    {
        private Random Random { get; set; } = new();

        private readonly float ParticleSpeed;

        private Point SceneSize { get; set; }
        private PrecipitationType PrecipitationType { get; set; }

        public List<PrecipitationParticle> BackgroundParticles { get; private set; } = [];
        public List<PrecipitationParticle> ForegroundParticles { get; private set; } = [];

        public PrecipitationManager(Point sceneSize, PrecipitationType precipitationType)
        {
            SceneSize = sceneSize;
            PrecipitationType = precipitationType;
            ParticleSpeed = precipitationType == PrecipitationType.Snow ? 8f : 16f;

            for (int i = 0; i < 300; i++)
            {
                UpdatePrecipitation();
            }
        }

        public void UpdatePrecipitation()
        {
            if (PrecipitationType == PrecipitationType.None) return;

            BackgroundParticles.RemoveAll(particle => particle.Position.Y > SceneSize.Y || particle.Position.X > SceneSize.X);
            ForegroundParticles.RemoveAll(particle => particle.Position.Y > SceneSize.Y || particle.Position.X > SceneSize.X);

            foreach (var particle in BackgroundParticles)
            {
                var xDirection = (float)(ParticleSpeed * Math.Cos(particle.Rotation));
                var yDirection = (float)(ParticleSpeed * Math.Sin(particle.Rotation));
                particle.Position = new Vector2(
                    particle.Position.X + (float)(ParticleSpeed * Math.Cos(particle.Rotation)),
                    particle.Position.Y + (float)(ParticleSpeed * Math.Sin(particle.Rotation))
                );
            }

            foreach (var particle in ForegroundParticles)
            {
                particle.Position = new Vector2(
                    particle.Position.X + (float)(ParticleSpeed * Math.Cos(particle.Rotation)),
                    particle.Position.Y + (float)(ParticleSpeed * Math.Sin(particle.Rotation))
                );
            }

            if (PrecipitationType == PrecipitationType.Rain && SceneSize.Y > 1620)
            {
                CreateNewParticle(false);
                CreateNewParticle(false);
                CreateNewParticle(true);
                CreateNewParticle(true);
            }
            
            if (PrecipitationType == PrecipitationType.Rain && SceneSize.Y > 1080)
            {
                CreateNewParticle(false);
                CreateNewParticle(true);
            }

            CreateNewParticle(false);
            CreateNewParticle(true);
        }

        private void CreateNewParticle(bool foreground)
        {
            var rotation = PrecipitationType == PrecipitationType.Rain ? 1.5f : 1f;

            if (PrecipitationType == PrecipitationType.Snow)
            {
                rotation += (((float)Random.NextDouble() - 0.5f) / 2f);
            }

            var x = Random.Next(-(int)(SceneSize.Y / 2.5f), SceneSize.X);

            int sizeX = 0, sizeY = 0;

            if (PrecipitationType == PrecipitationType.Snow)
            {
                sizeX = Random.Next(6, 12);
                sizeY = sizeX;
            }
            else if (PrecipitationType == PrecipitationType.Rain)
            {
                sizeX = 8;
                sizeY = 32;
            }
            
            if (foreground)
            {
                ForegroundParticles.Add(new PrecipitationParticle(PrecipitationType)
                {
                    Position = new Vector2(x, 0),
                    Rotation = rotation,
                    Size = new Point(sizeX, sizeY),
                    Foreground = foreground
                });
            }
            else
            {
                BackgroundParticles.Add(new PrecipitationParticle(PrecipitationType)
                {
                    Position = new Vector2(x, 0),
                    Rotation = rotation,
                    Size = new Point(sizeX, sizeY),
                    Foreground = foreground
                });
            }
            
        }
    }
}
