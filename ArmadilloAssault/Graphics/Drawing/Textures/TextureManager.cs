using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmadilloAssault.Graphics.Drawing.Textures
{
    public static class TextureManager
    {
        private static Dictionary<TextureName, Texture2D> _textures;

        public static void LoadContent(ContentManager contentManager)
        {
            _textures = [];

            _textures.Add(TextureName.logo, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "logo")));

            _textures.Add(TextureName.cursor, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "cursor")));
            _textures.Add(TextureName.crosshair, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "crosshair")));
            _textures.Add(TextureName.white_pixel, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "white_pixel")));

            foreach (var avatar in Enum.GetValues<AvatarType>())
            {
                _textures.Add(Enum.Parse<TextureName>(avatar.ToString().ToLower()), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Avatars", avatar.ToString().ToLower())));

                var selectName = Enum.Parse<TextureName>(avatar.ToString().ToLower()) + "_select";
                _textures.Add(Enum.Parse<TextureName>(selectName), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Avatar_Selection", selectName)));

                _textures.Add(Enum.Parse<TextureName>(avatar.ToString().ToLower() + "_white"), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Avatars", "White", avatar.ToString().ToLower())));
            }

            _textures.Add(TextureName.loading_spinner, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "loading_spinner")));
            _textures.Add(TextureName.clouds, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "clouds")));
            _textures.Add(TextureName.crates, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "crates")));
            _textures.Add(TextureName.crates_parachuting, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "crates_parachuting")));
            _textures.Add(TextureName.smoke, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "smoke")));

            _textures.Add(TextureName.pistol, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "pistol")));
            _textures.Add(TextureName.assault, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "assault")));
            _textures.Add(TextureName.shotgun, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "shotgun")));
            _textures.Add(TextureName.sniper, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", "sniper")));

            _textures.Add(TextureName.desert_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "desert_background")));
            _textures.Add(TextureName.mountain_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "mountain_background")));
            _textures.Add(TextureName.volcano_background, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "volcano_background")));
            _textures.Add(TextureName.floating_fortress, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "floating_fortress_background")));

            _textures.Add(TextureName.gusty_gorge_preview, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Level_Preview", "gusty_gorge")));
            _textures.Add(TextureName.sunken_sands_preview, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Level_Preview", "sunken_sands")));
            _textures.Add(TextureName.molten_mountain_preview, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Level_Preview", "molten_mountain")));

            _textures.Add(TextureName.lava_flow, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "lava_flow")));

            _textures.Add(TextureName.bullet_box, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "bullet_box")));
            _textures.Add(TextureName.bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "bullet")));
            _textures.Add(TextureName.shotgun_bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "shotgun_bullet")));
            _textures.Add(TextureName.rain, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "rain")));
            _textures.Add(TextureName.snowball, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "snowball")));

            _textures.Add(TextureName.test_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "test_tileset")));
            _textures.Add(TextureName.desert_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "desert_tileset")));
            _textures.Add(TextureName.volcano_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "volcano_tileset")));
            _textures.Add(TextureName.floating_island_tileset, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", "floating_island_tileset")));

            _textures.Add(TextureName.muzzle_flash_small, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "muzzle_flash_small")));
            _textures.Add(TextureName.muzzle_flash_large, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "muzzle_flash_large")));
            _textures.Add(TextureName.dust_cloud, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "dust_cloud")));
            _textures.Add(TextureName.blood_splatter, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "blood_splatter")));
            _textures.Add(TextureName.ricochet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Effects", "ricochet")));
        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }
    }
}
