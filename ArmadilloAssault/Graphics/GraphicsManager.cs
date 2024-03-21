using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ArmadilloAssault.Graphics
{
    public static class GraphicsManager
    {
        private static GraphicsDeviceManager _graphicsDeviceManager;

        private static GraphicsDevice _graphicsDevice;
        private static GameWindow _gameWindow;

        public static Point ScreenCenter => new(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

        public static bool IsFullscreen => _graphicsDeviceManager.IsFullScreen;

        public static bool IsBorderless => _gameWindow.IsBorderless;

        public static void Initialize(Engine engine)
        {
            _graphicsDeviceManager = new(engine)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                IsFullScreen = true
            };

            _gameWindow = engine.Window;

            _graphicsDeviceManager.ApplyChanges();
        }

        public static void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;

            DrawingManager.LoadContent(graphicsDevice);
            DrawingHelper.LoadContent(content);
            TextureManager.LoadContent(content);

            AvatarDrawingHelper.Initialize();
        }

        public static void Clear()
        {
            if (GameStateManager.State == State.Menu)
            {
                _graphicsDevice.Clear(Color.Black);
            }
            else
            {
                _graphicsDevice.Clear(Color.CornflowerBlue);
            }
        }

        public static void Clear(Color color)
        {
            _graphicsDevice.Clear(color);
        }

        public static void ToggleBorderless()
        {
            _gameWindow.IsBorderless = !_gameWindow.IsBorderless;
        }

        public static void ToggleFullscreen()
        {
            _graphicsDeviceManager.IsFullScreen = !_graphicsDeviceManager.IsFullScreen;
            _graphicsDeviceManager.ApplyChanges();
        }
    }
}
