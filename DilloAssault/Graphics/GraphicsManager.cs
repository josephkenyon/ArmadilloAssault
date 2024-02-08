using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DilloAssault.Graphics
{
    public static class GraphicsManager
    {
        private static GraphicsDeviceManager _graphicsDeviceManager;

        private static GraphicsDevice _graphicsDevice;

        public static void Initialize(Engine engine)
        {
            _graphicsDeviceManager = new(engine)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            _graphicsDeviceManager.IsFullScreen = true;

            _graphicsDeviceManager.ApplyChanges();
        }

        public static void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;

            DrawingManager.LoadContent(graphicsDevice);
            DrawingHelper.LoadContent(content);
            TextureManager.LoadContent(content);
        }

        public static void Clear()
        {
            _graphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}
