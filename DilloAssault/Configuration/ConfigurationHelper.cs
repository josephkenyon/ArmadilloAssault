﻿using DilloAssault.Assets;
using DilloAssault.Configuration.Json;
using DilloAssault.Configuration.Json.Avatars;
using DilloAssault.Configuration.Json.Scenes;
using DilloAssault.GameState.Battle.Avatars;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DilloAssault.Configuration
{
    public static class ConfigurationHelper
    {
        public static JsonSerializerSettings JsonSerializerSettings => new() { TypeNameHandling = TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public static string GetConfigurationPath(string folderName) => Path.Combine(ConfigurationManager.ContentManager.RootDirectory, "Configuration", folderName);

        public static Rectangle GetRectangle(RectangleJson rectangleJson) => new Rectangle(rectangleJson.X, rectangleJson.Y, rectangleJson.Width, rectangleJson.Height);
        public static List<Rectangle> GetHurtBoxes(HurtBoxListJson collisionBoxListJson)
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
                { Animation.Resting, new AnimationJson { FrameCount = 1, X = 0, Y = 1 } },
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
                    Texture = tileList.Tiles.First().TextureName.ToString(),
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

            var collisionBoxListJson = new HurtBoxListJson
            {
                X = [],
                Y = [],
                Width = [],
                Height = []
            };

            foreach (var box in scene.CollisionBoxes)
            {
                collisionBoxListJson.X.Add(box.X);
                collisionBoxListJson.Y.Add(box.Y);
                collisionBoxListJson.Width.Add(box.Width);
                collisionBoxListJson.Height.Add(box.Height);
            }

            json.CollisionBoxes = collisionBoxListJson;

            return json;
        }
    }
}
