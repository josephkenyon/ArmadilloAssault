﻿using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using DilloAssault.Configuration.Scenes;
using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Weapons;
using DilloAssault.Configuration.Effects;

namespace DilloAssault.Configuration
{
    public static class ConfigurationManager
    {
        public static ContentManager ContentManager { get; set; }


        private static Dictionary<string, SceneJson> _sceneConfigurations;
        private static Dictionary<AvatarType, AvatarJson> _avatarConfigurations;
        private static Dictionary<WeaponType, WeaponJson> _weaponConfigurations;
        private static Dictionary<EffectType, EffectJson> _effectConfigurations;

        public static void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;

            LoadScenes();
            LoadAvatars();
            LoadWeapons();
            LoadEffects();
        }

        private static void LoadEffects()
        {

            _effectConfigurations = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Effects"))
                .Where(file => file.EndsWith(".json"));

            var fileName = fileNames.First();

            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                var effectConfigurations = JsonConvert.DeserializeObject<List<EffectJson>>(json);

                effectConfigurations.ForEach(effect =>
                {
                    _effectConfigurations.Add(effect.Type, effect);
                });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);
            }
        }

        private static void LoadWeapons()
        {

            _weaponConfigurations = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Weapons"))
                .Where(file => file.EndsWith(".json"));

            var fileName = fileNames.First();
    
            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                var weaponConfigurations = JsonConvert.DeserializeObject<List<WeaponJson>>(json);

                weaponConfigurations.ForEach(weapon =>
                {
                    _weaponConfigurations.Add(weapon.Type, weapon);
                });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);
            }
        }

        private static void LoadAvatars()
        {

            _avatarConfigurations = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Avatars"))
                .Where(file => file.EndsWith(".json"));

            foreach (var fileName in fileNames)
            {
                using StreamReader r = new(fileName);

                string json = r.ReadToEnd();

                try
                {
                    var avatarJson = JsonConvert.DeserializeObject<AvatarJson>(json);

                    var avatarName = Path.GetFileNameWithoutExtension(fileName);

                    var avatarType = string.Concat(avatarName[0].ToString().ToUpper(), avatarName.AsSpan(1));

                    _avatarConfigurations.Add(Enum.Parse<AvatarType>(avatarType), avatarJson);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message, e);
                }
            }
        }

        private static void LoadScenes()
        {
            _sceneConfigurations = [];

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

                    _sceneConfigurations.Add(Path.GetFileNameWithoutExtension(fileName), sceneJson);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message, e);
                }
            }
        }

        public static SceneJson GetSceneConfiguration()
        {
            return _sceneConfigurations.Values.First();
        }

        public static AvatarJson GetAvatarConfiguration(AvatarType type)
        {
            return _avatarConfigurations[type];
        }

        public static WeaponJson GetWeaponConfiguration(WeaponType weaponType)
        {
            return _weaponConfigurations[weaponType];
        }

        public static EffectJson GetEffectConfiguration(EffectType effectType)
        {
            return _effectConfigurations[effectType];
        }
    }
}
