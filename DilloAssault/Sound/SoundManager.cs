using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Weapons;
using DilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DilloAssault.Sound
{
    public static class SoundManager
    {
        private static Dictionary<MusicSong, Song> _musicSongs;
        private static Dictionary<MenuSound, SoundEffect> _menuSounds;
        private static Dictionary<BattleSound, SoundEffect> _battleSounds;
        private static Dictionary<WeaponType, SoundEffect> _weaponSounds;
        private static Dictionary<AvatarType, Dictionary<AvatarSound, SoundEffect>> _avatarSounds;

        private static readonly float SoundScaler = 0.6f;

        private static SoundFrame _soundFrame;

        private static SoundEffectInstance _reloadInstance;
        private static SoundEffectInstance _reloadEndInstance;

        public static void LoadContent(ContentManager contentManager)
        {
            MediaPlayer.Volume = SoundScaler * 0.65f;

            LoadMusicSongs(contentManager);
            LoadMenuSounds(contentManager);
            LoadBattleSounds(contentManager);
            LoadWeaponSounds(contentManager);
            LoadAvatarSounds(contentManager);
        }

        private static void LoadMusicSongs(ContentManager contentManager)
        {
            _musicSongs = [];

            foreach (var song in Enum.GetValues<MusicSong>())
            {
                try
                {
                    var path = Path.Combine("Sound", "Music", song.ToString());
                    _musicSongs.Add(song, contentManager.Load<Song>(path));
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }
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

        public static void PlayMusic(MusicSong musicSong)
        {
            MediaPlayer.Play(_musicSongs[musicSong]);
            MediaPlayer.IsRepeating = true;
        }

        public static void PlayMenuSound(MenuSound menuSound)
        {
            _menuSounds[menuSound].Play();
        }

        public static void QueueBattleSound(BattleSound battleSound)
        {
            _soundFrame ??= new SoundFrame();

            if (!_soundFrame.BattleSounds.Contains(battleSound))
            {
                _soundFrame.BattleSounds.Add(battleSound);
            }
        }

        public static void QueueWeaponSound(WeaponType weaponType)
        {
            _soundFrame ??= new SoundFrame();

            if (!_soundFrame.WeaponSounds.Contains(weaponType))
            {
                _soundFrame.WeaponSounds.Add(weaponType);
            }
        }

        public static void QueueAvatarSound(AvatarType avatarType, AvatarSound avatarSound)
        {
            _soundFrame ??= new SoundFrame();
            if (!_soundFrame.AvatarSounds.Any(sound => sound.Key == avatarType && sound.Value == avatarSound))
            {
                _soundFrame.AvatarSounds.Add(new(avatarType, avatarSound));
            }
        }

        public static void CancelReloadSoundEffects()
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.CancelReloadSound = true;
        }

        public static void PlaySounds(SoundFrame soundFrame)
        {
            if (soundFrame != null)
            {
                if (soundFrame.CancelReloadSound)
                {
                    CancelReloadInstances();
                }

                foreach (var battleSound in soundFrame.BattleSounds)
                {
                    if (battleSound == BattleSound.reload)
                    {
                        _reloadInstance = _battleSounds[battleSound].CreateInstance();
                        _reloadInstance.Volume = SoundScaler;
                        _reloadInstance.Play();
                    }
                    else if (battleSound == BattleSound.reload_end)
                    {
                        _reloadEndInstance = _battleSounds[battleSound].CreateInstance();
                        _reloadEndInstance.Volume = SoundScaler;
                        _reloadEndInstance.Play();

                        CancelReloadInstance();
                    }
                    else
                    {
                        _battleSounds[battleSound].Play(SoundScaler, 0f, 0f);
                    }
                }

                foreach (var weaponSound in soundFrame.WeaponSounds.Distinct())
                {
                    _weaponSounds[weaponSound].Play(0.5f * SoundScaler, 0f, 0f);
                }

                foreach (var avatarSound in soundFrame.AvatarSounds.Distinct())
                {
                    _avatarSounds[avatarSound.Key][avatarSound.Value].Play(0.5f * SoundScaler, 0f, 0f);
                }
            }
        }

        private static void CancelReloadInstances()
        {
            if (_reloadEndInstance != null && _reloadEndInstance.State == SoundState.Playing)
            {
                _reloadEndInstance.Stop();
                _reloadEndInstance = null;
            }

            CancelReloadInstance();
        }

        private static void CancelReloadInstance()
        {
            if (_reloadInstance != null && _reloadInstance.State == SoundState.Playing)
            {
                _reloadInstance.Stop();
                _reloadInstance = null;
            }
        }

        public static void AddSounds(BattleFrame battleFrame)
        {
            battleFrame.SoundFrame = _soundFrame;
            _soundFrame = null;
        }
    }
}
