using DilloAssault.Assets;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DilloAssault.Graphics.Drawing.Textures
{
    public static class TextureManager
    {
        private static Dictionary<TextureName, Texture2D> _textures;

        public static void LoadContent(ContentManager contentManager)
        {
            _textures = [];

            foreach (var texture in Enum.GetValues(typeof(TextureName)).Cast<TextureName>())
            {
                _textures.Add(texture, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", texture.ToString())));
            }
        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }
    }
}
