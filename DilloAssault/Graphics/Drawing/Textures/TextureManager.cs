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


            _textures.Add(TextureName.arthur, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Arthur", "arthur")));
            _textures.Add(TextureName.arthur_head, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Arthur", "arthur_head")));
            _textures.Add(TextureName.arthur_right_arm, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Arthur", "arthur_right_arm")));
            _textures.Add(TextureName.arthur_left_arm, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Arthur", "arthur_left_arm")));

            _textures.Add(TextureName.pistol, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "pistol")));
            _textures.Add(TextureName.assault, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "assault")));
            _textures.Add(TextureName.shotgun, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "shotgun")));
            _textures.Add(TextureName.sniper, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "sniper")));

            _textures.Add(TextureName.forest_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "forest_background")));
            _textures.Add(TextureName.jungle_level_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "jungle_level_background")));
            _textures.Add(TextureName.jungle_level_foreground, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "jungle_level_foreground")));

            _textures.Add(TextureName.test_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "test_tileset")));

            _textures.Add(TextureName.muzzle_flash_small, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "muzzle_flash_small")));
        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }
    }
}
