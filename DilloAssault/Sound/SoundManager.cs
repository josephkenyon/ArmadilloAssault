using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Weapons;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace DilloAssault.Sound
{
    public static class SoundManager
    {
        private static Dictionary<MenuSound, SoundEffect> _menuSounds;
        private static Dictionary<BattleSound, SoundEffect> _battleSounds;
        private static Dictionary<WeaponType, SoundEffect> _weaponSounds;
        private static Dictionary<AvatarType, Dictionary<AvatarSound, SoundEffect>> _avatarSounds;

        public static void LoadContent(ContentManager contentManager)
        {
            LoadMenuSounds(contentManager);
            LoadBattleSounds(contentManager);
            LoadWeaponSounds(contentManager);
            LoadAvatarSounds(contentManager);
        }

        private static void LoadMenuSounds(ContentManager contentManager)
        {
            _menuSounds = [];

            foreach (var sound in Enum.GetValues<MenuSound>())
            {
                try
                {
                    var path = Path.Combine("Sound", "Menu", sound.ToString());
                    _menuSounds.Add(sound, contentManager.Load<SoundEffect>(path));
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }
        }

        private static void LoadBattleSounds(ContentManager contentManager)
        {
            _battleSounds = [];

            foreach (var sound in Enum.GetValues<BattleSound>())
            {
                try
                {
                    var path = Path.Combine("Sound", "Battle", sound.ToString());
                    _battleSounds.Add(sound, contentManager.Load<SoundEffect>(path));
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }
        }

        private static void LoadWeaponSounds(ContentManager contentManager)
        {
            _weaponSounds = [];

            foreach (var weaponType in Enum.GetValues<WeaponType>())
            {
                try
                {
                    var path = Path.Combine("Sound", "Weapon", weaponType.ToString().ToLower());
                    _weaponSounds.Add(weaponType, contentManager.Load<SoundEffect>(path));
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }
        }

        private static void LoadAvatarSounds(ContentManager contentManager)
        {
            _avatarSounds = [];

            foreach (var avatarType in Enum.GetValues<AvatarType>())
            {
                var soundDictionary = new Dictionary<AvatarSound, SoundEffect>();

                foreach (var sound in Enum.GetValues<AvatarSound>())
                {
                    try
                    {
                        var path = Path.Combine("Sound", avatarType.ToString(), $"{avatarType.ToString().ToLower()}_{sound.ToString().ToLower()}");
                        soundDictionary.Add(sound, contentManager.Load<SoundEffect>(path));
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(ex);
                    }
                }

                _avatarSounds.Add(avatarType, soundDictionary);
            }
        }

        public static void PlayMenuSound(MenuSound menuSound)
        {
            _menuSounds[menuSound].Play();
        }

        public static void PlayBattleSound(BattleSound battleSound)
        {
            _battleSounds[battleSound].Play();
        }

        public static void PlayWeaponSound(WeaponType weaponType)
        {
            _weaponSounds[weaponType].Play(0.5f, 0f, 0f);
        }

        public static void PlayAvatarSound(AvatarType avatarType, AvatarSound avatarSound)
        {
            _avatarSounds[avatarType][avatarSound].Play(0.65f, 0f, 0f);
        }
    }
}
