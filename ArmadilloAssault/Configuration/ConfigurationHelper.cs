using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArmadilloAssault.Configuration
{
    public static class ConfigurationHelper
    {
        public static JsonSerializerSettings JsonSerializerSettings => new() { TypeNameHandling = TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public static string GetConfigurationPath(string folderName) => Path.Combine(ConfigurationManager.ContentManager.RootDirectory, "Configuration", folderName);

        public static Rectangle GetRectangle(RectangleJson rectangleJson) => new(rectangleJson.X, rectangleJson.Y, rectangleJson.Width, rectangleJson.Height);
        public static List<Rectangle> GetRectangles(RectangleListJson collisionBoxListJson)
        {
            List<Rectangle> collisionBoxesList = [];

            for (var i = 0; i < collisionBoxListJson.X.Count; i++)
            {
                var rectangle = new Rectangle(collisionBoxListJson.X[i], collisionBoxListJson.Y[i], collisionBoxListJson.Width[i], collisionBoxListJson.Height[i]);

                collisionBoxesList.Add(rectangle);
            }

            return collisionBoxesList;
        }

        public static Dictionary<Animation, AnimationJson> GetAnimations(AnimationsJson animationsJson)
        {
            var animations = new Dictionary<Animation, AnimationJson>
            {
                { Animation.Dead, new AnimationJson { FrameCount = 1, X = 5, Y = 0 } },
                { Animation.Resting, new AnimationJson { FrameCount = 1, X = 0, Y = 3 } },
                { Animation.Running, animationsJson.Running },
                { Animation.Rolling, animationsJson.Spinning },
                { Animation.Jumping, animationsJson.Jumping },
                { Animation.Spinning, animationsJson.Spinning },
                { Animation.Falling, animationsJson.Falling },
            };

            return animations;
        }

        public static SceneJson GetSceneJson(Scene scene)
        {
            var json = new SceneJson
            {
                TileLists = []
            };

            foreach (var tileList in scene.TileLists)
            {
                var tileListJson = new TileListJson
                {
                    Z = tileList.Z,
                    X = [],
                    Y = [],
                    SpriteX = [],
                    SpriteY = []
                };

                foreach (var tile in tileList.Tiles)
                {
                    tileListJson.X.Add(tile.Position.X);
                    tileListJson.Y.Add(tile.Position.Y);
                    tileListJson.SpriteX.Add(tile.SpriteLocation.X);
                    tileListJson.SpriteY.Add(tile.SpriteLocation.Y);
                }

                json.TileLists.Add(tileListJson);
            }

            var collisionBoxListJson = new RectangleListJson
            {
                X = [],
                Y = [],
                Width = [],
                Height = []
            };

            foreach (var box in scene.CollisionBoxes.Where(box => box.Width > 1 && box.Height > 1))
            {
                collisionBoxListJson.X.Add(box.X);
                collisionBoxListJson.Y.Add(box.Y);
                collisionBoxListJson.Width.Add(box.Width);
                collisionBoxListJson.Height.Add(box.Height);
            }

            json.CollisionBoxes = collisionBoxListJson;

            json.BackgroundTexture = scene.BackgroundTexture;
            json.TilesetTexture = scene.TilesetTexture;

            json.StartingPositions = scene.StartingPositions.Select(pos => PointJson.CreateFrom((pos / DrawingHelper.FullTileSize).ToPoint())).ToList();
            json.BackgroundColor = scene.BackgroundColorJson;

            if (scene.CapturePoint != null)
            {
                json.CapturePoint = RectangleJson.CreateFrom((Rectangle)scene.CapturePoint, 1f / DrawingHelper.FullTileSize);
            }

            json.WrapY = scene.WrapY;

            json.Size = new PointJson(scene.Size.X, scene.Size.Y);

            return json;
        }
    }
}
