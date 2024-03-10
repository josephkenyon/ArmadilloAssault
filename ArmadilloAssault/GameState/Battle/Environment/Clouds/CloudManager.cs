using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Environment.Clouds
{
    public class CloudManager
    {
        private int CloudSpawnRate => HighCloudsOnly ? 750 : 300;
        public static readonly int CloudSpriteSize = 256;
        public static readonly float CloudSpeed = 1.5f;

        public List<Cloud> Clouds { get; private set; }
        private Random Random { get; set; }

        private List<int> AllowedXs { get; set; }
        private int AllowedXsSize { get; set; }
        private int SpeedSign { get; set; }
        private int LastY { get; set; }
        private int LastSprite { get; set; }
        private int FramesSinceLastCloud { get; set; }
        private bool HighCloudsOnly { get; set; }
        private Point SceneSize { get; set; }

        public CloudManager(bool highCloudsOnly, Point? sceneSize = null)
        {
            SceneSize = sceneSize ?? new Point(1920, 1080);
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

        public void UpdateClouds()
        {
            Clouds.RemoveAll(cloud => (cloud.Position.X > (SceneSize.X + (cloud.Size.X * 2))) || cloud.Position.X < (0 - (cloud.Size.X * 2)));

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

        private void CreateNewCloud(bool anyX, bool foreground)
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
                x = AllowedXs[xGeneration] * (SceneSize.X / AllowedXsSize) - size.X;

                if (AllowedXs.Count < 1)
                {
                    AllowedXs = [0, 1, 2, 3, 4, 8, 9, 10, 11, 12];
                }
            }
            else
            {
                x = speed < 0 ? SceneSize.X : 0 - size.X;
            }

            // Speed Sign
            if (anyX)
            {
                if ((x + (size.X / 2)) > (SceneSize.X / 2))
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

            y = (int)((y * (SceneSize.Y / 16.875)) - (size.Y / 2));

            // Add Cloud
            Clouds.Add(new Cloud
            {
                Position = new Vector2((float)x, y),
                Size = size,
                SpriteLocation = new(spriteX, spriteY),
                Speed = speed,
                Foreground = foreground
            });
        }
    }
}
