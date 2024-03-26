using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Configuration.Weapons;
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

            _textures.Add(TextureName.bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "bullet")));
            _textures.Add(TextureName.bullet_box, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "bullet_box")));
            _textures.Add(TextureName.crosshair, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "crosshair")));
            _textures.Add(TextureName.crown, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Crown")));
            _textures.Add(TextureName.cursor, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "cursor")));
            _textures.Add(TextureName.lava_flow, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "lava_flow")));
            _textures.Add(TextureName.logo, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "logo")));
            _textures.Add(TextureName.rain, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "rain")));
            _textures.Add(TextureName.shotgun_bullet, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "shotgun_bullet")));
            _textures.Add(TextureName.skull, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "skull")));
            _textures.Add(TextureName.snowball, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "snowball")));
            _textures.Add(TextureName.white_pixel, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "white_pixel")));

            _textures.Add(TextureName.clouds, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Effects", "clouds")));

            _textures.Add(TextureName.crates, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Crates", "crates")));
            _textures.Add(TextureName.crates_ballooning, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Crates", "crates_ballooning")));
            _textures.Add(TextureName.crates_parachuting, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Crates", "crates_parachuting")));

            _textures.Add(TextureName.flag, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Items", "flag")));

            _textures.Add(TextureName.loading_spinner, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "loading_spinner")));

            foreach (var avatar in Enum.GetValues<AvatarType>())
            {
                var selectName = Enum.Parse<TextureName>(avatar.ToString().ToLower()) + "_select";

                _textures.Add(Enum.Parse<TextureName>(avatar.ToString().ToLower()), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Avatars", avatar.ToString().ToLower())));
                _textures.Add(Enum.Parse<TextureName>(selectName), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Avatar_Selection", selectName)));
                _textures.Add(Enum.Parse<TextureName>(avatar.ToString().ToLower() + "_white"), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Avatars", "White", avatar.ToString().ToLower())));
            }

            foreach (var effect in Enum.GetValues<EffectType>())
            {
                var effectString = effect.ToString();

                _textures.Add(Enum.Parse<TextureName>(effectString), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Sprites", "Effects", effectString)));
            }

            foreach (var weaponType in Enum.GetValues<WeaponType>())
            {
                var weaponTypeString = weaponType.ToString().ToLower();

                _textures.Add(Enum.Parse<TextureName>(weaponTypeString), contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Weapons", weaponTypeString)));
            }

            foreach (var backgroundTextureNames in BackgroundTextureNames)
            {
                _textures.Add(backgroundTextureNames, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Textures", "Backgrounds", backgroundTextureNames.ToString())));
            }

            foreach (var tileSetTextureNames in TileSetTextureNames)
            {
                _textures.Add(tileSetTextureNames, contentManager.Load<Texture2D>(Path.Combine("Graphics", "Tilesets", tileSetTextureNames.ToString())));
            }
        }

        public static Texture2D GetTexture(TextureName textureName)
        {
            return _textures[textureName];
        }

        private static readonly List<TextureName> TileSetTextureNames = [
            TextureName.gusty_gorge_tileset,
            TextureName.desert_tileset,
            TextureName.volcano_tileset,
            TextureName.floating_island_tileset,
            TextureName.snow_tileset,
            TextureName.haunted_house_tileset
        ];

        private static readonly List<TextureName> BackgroundTextureNames = [
            TextureName.desert_background,
            TextureName.mountain_background,
            TextureName.volcano_background,
            TextureName.floating_fortress_background,
            TextureName.cloud_background,
            TextureName.snow_background,
            TextureName.moon_sky_background,
            TextureName.haunted_house_background,
            TextureName.gloomy_glade_background
        ];
    }
}
