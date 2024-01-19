using Newtonsoft.Json;
using System.IO;

namespace DilloAssault.Configuration
{
    public static class ConfigurationHelper
    {
        public static JsonSerializerSettings JsonSerializerSettings => new() { TypeNameHandling = TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public static string ScenesPath => Path.Combine(ConfigurationManager.ContentManager.RootDirectory, "Configuration", "Scenes");
    }
}
