using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Textures;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArmadilloAssault.Graphics
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
                PreferredBackBufferHeight = 1080,
                IsFullScreen = true
            };

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
            if (GameStateManager.State == State.Menu)
            {
                _graphicsDevice.Clear(Color.Black);
            }
            else
            {
                _graphicsDevice.Clear(Color.CornflowerBlue);
            }
        }

        public static void SetBattleCursor()
        {
            Mouse.SetCursor(MouseCursor.FromTexture2D(TextureManager.GetTexture(TextureName.crosshair), 0, 0));
        }

        public static void ToggleFullscreen()
        {
            _graphicsDeviceManager.IsFullScreen = !_graphicsDeviceManager.IsFullScreen;
            _graphicsDeviceManager.ApplyChanges();
        }

        public static void SetMenuCursor()
        {
            Mouse.SetCursor(MouseCursor.Arrow);
        }
    }
}
