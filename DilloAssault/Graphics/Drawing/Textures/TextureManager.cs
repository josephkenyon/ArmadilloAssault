using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace DilloAssault.Graphics.Drawing.Textures
{
    public static class TextureManager
    {
        private static Dictionary<TextureName, Texture2D> _textures;

        public static void LoadContent(ContentManager contentManager)
        {
            _textures = [];

            _textures.Add(TextureName.white_pixel, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "white_pixel")));

            _textures.Add(TextureName.test_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "test_tileset")));

        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }
    }
}
