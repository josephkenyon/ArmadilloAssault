using DilloAssault.Assets;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DilloAssault.Configuration.Scenes
{
    internal static class SceneManager
    {
        private static Dictionary<string, Scene> _scenes;

        public static void LoadContent(ContentManager contentManager)
        {
            _scenes = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.ScenesPath)
                .Where(file => file.EndsWith(".json"));
            
            foreach (var fileName in fileNames)
            {
                using StreamReader r = new(fileName);

                string json = r.ReadToEnd();

                try
                {
                    var sceneJson = JsonConvert.DeserializeObject<SceneJson>(json);
                    var scene = new Scene(sceneJson);

                    _scenes.Add(Path.GetFileNameWithoutExtension(fileName), scene);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message, e);
                }
            }
        }

        public static Scene GetSceneConfiguration()
        {
            return _scenes.Values.First();
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

            return json;
        }
    }
}
