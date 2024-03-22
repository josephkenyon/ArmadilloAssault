using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Configuration.Web;

namespace ArmadilloAssault.Configuration
{
    public static class ConfigurationManager
    {
        public static ContentManager ContentManager { get; set; }

        private static WebJson _webJson;

        private static Dictionary<string, MenuJson> _menuConfigurations;
        private static Dictionary<string, List<string>> _toolTips;
        private static Dictionary<string, SceneJson> _sceneConfigurations;
        private static Dictionary<AvatarType, AvatarJson> _avatarConfigurations;
        private static Dictionary<WeaponType, WeaponJson> _weaponConfigurations;
        private static Dictionary<EffectType, EffectJson> _effectConfigurations;
        private static Dictionary<ItemType, ItemJson> _itemConfigurations;

        public static Dictionary<string, SceneJson> SceneConfigurations => _sceneConfigurations;

        public static void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;

            LoadWeb();
            LoadMenus();
            LoadTooltips();
            LoadScenes();
            LoadAvatars();
            LoadWeapons();
            LoadEffects();
            LoadItems();
        }

        private static void LoadWeb()
        {
            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Web"))
                .Where(file => file.EndsWith("web.json"));

            var fileName = fileNames.First();

            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                _webJson = JsonConvert.DeserializeObject<WebJson>(json);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);

                _webJson = new WebJson();
            }
        }

        private static void LoadMenus()
        {
            _menuConfigurations = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Menus"))
                .Where(file => file.EndsWith("menus.json"));

            var fileName = fileNames.First();

            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                var menuJsons = JsonConvert.DeserializeObject<List<MenuJson>>(json);

                menuJsons.ForEach(menuJson =>
                {
                    _menuConfigurations.Add(menuJson.Name, menuJson);
                });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);
            }
        }

        private static void LoadTooltips()
        {
            _toolTips = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Menus"))
                .Where(file => file.EndsWith("tooltips.json"));

            var fileName = fileNames.First();

            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                var tooltipJsons = JsonConvert.DeserializeObject<List<TooltipJson>>(json);

                tooltipJsons.ForEach(json =>
                {
                    _toolTips.Add(json.Key, json.Texts);
                });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);
            }
        }

        public static void LoadScenes()
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

                    var avatarType = Enum.Parse<AvatarType>(string.Concat(avatarName[0].ToString().ToUpper(), avatarName.AsSpan(1)));

                    avatarJson.Type = avatarType;

                    _avatarConfigurations.Add(avatarType, avatarJson);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message, e);
                }
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

        private static void LoadItems()
        {
            _itemConfigurations = [];

            var fileNames = Directory
                .GetFiles(ConfigurationHelper.GetConfigurationPath("Items"))
                .Where(file => file.EndsWith("items.json"));

            var fileName = fileNames.First();

            using StreamReader r = new(fileName);

            string json = r.ReadToEnd();

            try
            {
                var itemJsons = JsonConvert.DeserializeObject<List<ItemJson>>(json);

                itemJsons.ForEach(itemJson =>
                {
                    _itemConfigurations.Add(itemJson.Type, itemJson);
                });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message, e);
            }
        }

        public static WebJson GetWebJson()
        {
            return _webJson;
        }

        public static MenuJson GetMenuConfiguration(string name)
        {
            return _menuConfigurations.Values.Single(screen => screen.Name == name);
        }

        public static List<string> GetToolTip(string name)
        {
            if (_toolTips.TryGetValue(name, out List<string> value))
            {
                return value;
            }

            return [];
        }

        public static SceneJson GetSceneConfiguration(string name)
        {
            if (!_sceneConfigurations.ContainsKey(name))
            {
                return new SceneJson();
            }

            return _sceneConfigurations[name];
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

        public static ItemJson GetItemConfiguration(ItemType itemType)
        {
            return _itemConfigurations[itemType];
        }
    }
}
