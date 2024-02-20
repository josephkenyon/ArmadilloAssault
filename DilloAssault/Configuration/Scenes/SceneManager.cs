using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArmadilloAssault.Configuration.Scenes
{
    public static class SceneManager
    {
        private static Dictionary<string, SceneJson> _sceneJsons;

        public static void LoadContent(ContentManager contentManager)
        {
            _sceneJsons = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Scenes"))
                .Where(file => file.EndsWith(".json"));

            foreach (var fileName in fileNames)
            {
                using StreamReader r = new(fileName);

                string json = r.ReadToEnd();

                try
                {
                    var sceneJson = JsonConvert.DeserializeObject<SceneJson>(json);

                    _sceneJsons.Add(Path.GetFileNameWithoutExtension(fileName), sceneJson);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message, e);
                }
            }
        }

        public static SceneJson GetSceneConfiguration()
        {
            return _sceneJsons.Values.First();
        }
    }
}
