using Microsoft.Xna.Framework.Content;
using DilloAssault.Configuration.Scenes;
using DilloAssault.Assets;

namespace DilloAssault.Configuration
{
    public static class ConfigurationManager
    {
        public static ContentManager ContentManager { get; set; }
        public static void LoadContent(ContentManager contentManager)
        {
            ContentManager = contentManager;

            SceneManager.LoadContent(contentManager);
        }

        public static Scene GetSceneConfiguration()
        {
            return SceneManager.GetSceneConfiguration();
        }
    }
}
