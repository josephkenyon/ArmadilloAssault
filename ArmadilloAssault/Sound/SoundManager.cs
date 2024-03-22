using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArmadilloAssault.Sound
{
    public static class SoundManager
    {
        private static readonly float scaler = 0.16f;

        private static Dictionary<MusicSong, Song> _musicSongs;
        private static Dictionary<MenuSound, SoundEffect> _menuSounds;
        private static Dictionary<BattleSound, SoundEffect> _battleSounds;
        private static Dictionary<WeaponType, SoundEffect> _weaponSounds;
        private static Dictionary<AvatarType, Dictionary<AvatarSound, SoundEffect>> _avatarSounds;

        private static float SoundScaler => SoundEffectsEnabled ? scaler : 0f;

        private static SoundFrame _soundFrame;

        public static bool MusicEnabled { get; private set; } = true;
        public static bool SoundEffectsEnabled { get; private set; } = true;

        private static SoundEffectInstance _reloadInstance;
        private static SoundEffectInstance _reloadEndInstance;

        public static void LoadContent(ContentManager contentManager)
        {
            MediaPlayer.Volume = SoundScaler * 0.4f;

            LoadMusicSongs(contentManager);
            LoadMenuSounds(contentManager);
            LoadBattleSounds(contentManager);
            LoadWeaponSounds(contentManager);
            LoadAvatarSounds(contentManager);
        }

        public static void ToggleMusic()
        {
            MusicEnabled = !MusicEnabled;
            
            MediaPlayer.Volume = scaler * (MusicEnabled ? 0.5f : 0f);
        }

        public static void ToggleSoundEffects()
        {
            SoundEffectsEnabled = !SoundEffectsEnabled;
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
            if (MusicEnabled)
            {
                MediaPlayer.Play(_musicSongs[musicSong]);
                MediaPlayer.IsRepeating = true;
            }
        }

        public static void PlayMenuSound(MenuSound menuSound)
        {
            _menuSounds[menuSound].Play(SoundScaler, 0f, 0f);
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
            _soundFrame.CancelReloudSound = true;
        }

        public static void PlaySounds(SoundFrame soundFrame)
        {
            if (soundFrame != null && !soundFrame.Played)
            {
                soundFrame.Played = true;

                if (soundFrame.CancelReloudSound)
                {
                    CancelReloadInstances();
                }

                if (SoundEffectsEnabled)
                {
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
                        var scaleFactor = (avatarSound.Key == AvatarType.Claus || (avatarSound.Key == AvatarType.Angie && avatarSound.Value == AvatarSound.Ready))
                            ? 1f : (avatarSound.Value == AvatarSound.Hurt || avatarSound.Value == AvatarSound.Dead ? 0.75f : 0.5f);
                        _avatarSounds[avatarSound.Key][avatarSound.Value].Play(scaleFactor * SoundScaler, 0f, 0f);
                    }
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

        public static void PushSounds(ISoundFrameContainer soundFrameContainer)
        {
            soundFrameContainer.SoundFrame = _soundFrame;
            _soundFrame = null;
        }
    }
}
