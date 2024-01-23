using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using DilloAssault.Configuration.Json.Scenes;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using DilloAssault.Configuration.Json.Avatars;

namespace DilloAssault.Configuration
{
    public static class ConfigurationManager
    {
        public static ContentManager ContentManager { get; set; }


        private static Dictionary<string, SceneJson> _sceneJsons;
        private static Dictionary<string, AvatarJson> _avatarJsons;

        public static void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;

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


            _avatarJsons = [];

            fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Avatars"))
                .Where(file => file.EndsWith(".json"));

            foreach (var fileName in fileNames)
            {
                using StreamReader r = new(fileName);

                string json = r.ReadToEnd();

                try
                {
                    var avatarJson = JsonConvert.DeserializeObject<AvatarJson>(json);

                    _avatarJsons.Add(Path.GetFileNameWithoutExtension(fileName), avatarJson);
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

        public static AvatarJson GetAvatarConfiguration()
        {
            return _avatarJsons.Values.First();
        }
    }
}
