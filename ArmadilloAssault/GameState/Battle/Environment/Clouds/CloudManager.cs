using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Environment.Clouds
{
    public static class CloudManager
    {
        private static int CloudSpawnRate => HighCloudsOnly ? 750 : 300;
        public static readonly int CloudSpriteSize = 256;
        public static readonly float CloudSpeed = 1.5f;


        public static List<Cloud> Clouds { get; private set; }
        private static Random Random { get; set; }

        private static List<int> AllowedXs { get; set; }
        private static int AllowedXsSize { get; set; }
        private static int SpeedSign { get; set; }
        private static int LastY { get; set; }
        private static int LastSprite { get; set; }
        private static int FramesSinceLastCloud { get; set; }
        private static bool HighCloudsOnly { get; set; }

        public static void Initialize(bool highCloudsOnly)
        {
            HighCloudsOnly = highCloudsOnly;
            AllowedXs = [0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12];
            AllowedXsSize = AllowedXs.Count;

            SpeedSign = -1;
            LastY = -1;
            FramesSinceLastCloud = CloudSpawnRate;
            LastSprite = 0;

            Clouds = [];

            Random = new();

            for (int i = 0; i < (HighCloudsOnly ? 4 : 9); i++)
            {
                CreateNewCloud(true, false);
                CreateNewCloud(true, true);
            }
        }

        public static void UpdateClouds()
        {
            Clouds.RemoveAll(cloud => (cloud.Position.X > (1920 + (cloud.Size.X * 2))) || cloud.Position.X < (0 - (cloud.Size.X * 2)));

            foreach (var cloud in Clouds)
            {
                cloud.Position = new Vector2(cloud.Position.X + cloud.Speed / 2f, cloud.Position.Y);
            }

            if (FramesSinceLastCloud == CloudSpawnRate)
            {
                CreateNewCloud(false, false);
                CreateNewCloud(false, true);
                FramesSinceLastCloud = 0;
            }
            else
            {
                FramesSinceLastCloud++;
            }
        }

        private static void CreateNewCloud(bool anyX, bool foreground)
        {
            // SpriteX
            var spriteX = LastSprite % 2;


            // SpriteY
            var spriteY = LastSprite / 2;

            LastSprite++;

            if (LastSprite == 8)
            {
                LastSprite = 0;
            }


            // Speed
            var speed = (float)Random.NextDouble() + CloudSpeed / 2;


            // Size
            var randomSize = 2f + 1f;
            var size = new Point((int)(CloudSpriteSize * randomSize), (int)(CloudSpriteSize * randomSize));


            // Speed Sign
            if (!anyX)
            {
                SpeedSign = SpeedSign == 0 ? 1 : 0;

                speed = SpeedSign == 1 ? -speed : speed;
            }

            // X
            var x = 0f;
            if (anyX)
            {
                var xGeneration = Random.Next(0, AllowedXs.Count);
                x = AllowedXs[xGeneration] * (1920 / AllowedXsSize) - size.X;

                if (AllowedXs.Count < 1)
                {
                    AllowedXs = [0, 1, 2, 3, 4, 8, 9, 10, 11, 12];
                }
            }
            else
            {
                x = speed < 0 ? 1920 : 0 - size.X;
            }

            // Speed Sign
            if (anyX)
            {
                if ((x + (size.X / 2)) > (1920 / 2))
                {
                    speed = -speed;
                }
            }

            // Y
            int y = LastY + 1;

            if (y > (HighCloudsOnly ? 5 : 12))
            {
                y = 0;
            }

            LastY = y;

            y = (y * 64) - (size.Y / 2);

            // Add Cloud
            Clouds.Add(new Cloud
            {
                Position = new Vector2((float)x, (float)y),
                Size = size,
                SpriteLocation = new(spriteX, spriteY),
                Speed = speed,
                Foreground = foreground
            });
        }
    }
}
