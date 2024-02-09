using DilloAssault.Configuration.Textures;
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

            _textures.Add(TextureName.arthur, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "arthur")));
            _textures.Add(TextureName.axel, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "axel")));

            _textures.Add(TextureName.clouds, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "clouds")));

            _textures.Add(TextureName.pistol, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "pistol")));
            _textures.Add(TextureName.assault, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "assault")));
            _textures.Add(TextureName.shotgun, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "shotgun")));
            _textures.Add(TextureName.sniper, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "sniper")));

            _textures.Add(TextureName.forest_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "forest_background")));
            _textures.Add(TextureName.jungle_level_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "jungle_level_background")));
            _textures.Add(TextureName.jungle_level_foreground, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "jungle_level_foreground")));
            _textures.Add(TextureName.mountain_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "mountain_background")));

            _textures.Add(TextureName.bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "bullet")));
            _textures.Add(TextureName.shotgun_bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "shotgun_bullet")));

            _textures.Add(TextureName.test_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "test_tileset")));

            _textures.Add(TextureName.muzzle_flash_small, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "muzzle_flash_small")));
            _textures.Add(TextureName.muzzle_flash_large, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "muzzle_flash_large")));
            _textures.Add(TextureName.dust_cloud, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "dust_cloud")));
            _textures.Add(TextureName.blood_splatter, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "blood_splatter")));
        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }
    }
}
