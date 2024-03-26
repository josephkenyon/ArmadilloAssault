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

        private static Dictionary<int, SoundEffectInstance> _reloadInstances = [];
        private static Dictionary<int, SoundEffectInstance> _reloadEndInstances = [];

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

        public static void QueueReloadSound(int index)
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.PlayerReloads ??= [];

            _soundFrame.PlayerReloads.Add(index);
        }

        public static void QueueReloadEndSound(int index)
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.PlayerReloadEnds ??= [];

            _soundFrame.PlayerReloadEnds.Add(index);
        }


        public static void QueueBattleSound(BattleSound battleSound)
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.BattleSounds ??= [];

            if (!_soundFrame.BattleSounds.Contains(battleSound))
            {
                _soundFrame.BattleSounds.Add(battleSound);
            }
        }

        public static void QueueWeaponSound(WeaponType weaponType)
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.WeaponSounds ??= [];

            if (!_soundFrame.WeaponSounds.Contains(weaponType))
            {
                _soundFrame.WeaponSounds.Add(weaponType);
            }
        }

        public static void QueueAvatarSound(AvatarType avatarType, AvatarSound avatarSound)
        {
            _soundFrame ??= new SoundFrame();
            _soundFrame.AvatarSounds ??= [];

            if (!_soundFrame.AvatarSounds.Any(sound => sound.Key == avatarType && sound.Value == avatarSound))
            {
                _soundFrame.AvatarSounds.Add(new(avatarType, avatarSound));
            }
        }

        public static void CancelReloadSoundEffects(int playerIndex)
        {
            _soundFrame ??= new();
            _soundFrame.CancelReloudSounds ??= [];

            _soundFrame.CancelReloudSounds.Add(playerIndex);
        }

        public static void PlaySounds()
        {
            PlaySounds(_soundFrame);
            _soundFrame = null;
        }

        public static void PlaySounds(SoundFrame soundFrame)
        {
            if (soundFrame != null)
            {
                soundFrame.CancelReloudSounds?.ForEach(CancelReloadEndInstance);

                if (SoundEffectsEnabled)
                {
                    soundFrame.PlayerReloads?.ForEach(index =>
                    {
                        CancelReloadInstance(index);

                        _reloadInstances[index] = _battleSounds[BattleSound.reload].CreateInstance();
                        _reloadInstances[index].Volume = SoundScaler;
                        _reloadInstances[index].Play();
                    });

                    soundFrame.PlayerReloadEnds?.ForEach(index =>
                    {
                        CancelReloadEndInstance(index);

                        _reloadInstances[index] = _battleSounds[BattleSound.reload_end].CreateInstance();
                        _reloadInstances[index].Volume = SoundScaler;
                        _reloadInstances[index].Play();
                    });

                    soundFrame.BattleSounds?.ForEach(battleSound => _battleSounds[battleSound].Play(SoundScaler, 0f, 0f));

                    soundFrame.WeaponSounds?.Distinct().ToList().ForEach(weaponSound => _weaponSounds[weaponSound].Play(0.5f * SoundScaler, 0f, 0f));

                    soundFrame.AvatarSounds?.Distinct().ToList().ForEach(avatarSound =>
                    {
                        var scaleFactor = (avatarSound.Key == AvatarType.Claus || (avatarSound.Key == AvatarType.Angie && avatarSound.Value == AvatarSound.Ready))
                            ? 1f : (avatarSound.Value == AvatarSound.Hurt || avatarSound.Value == AvatarSound.Dead ? 0.75f : 0.5f);

                        _avatarSounds[avatarSound.Key][avatarSound.Value].Play(scaleFactor * SoundScaler, 0f, 0f);
                    });
                }
            }
        }

        private static void CancelReloadEndInstance(int playerIndex)
        {
            if (_reloadEndInstances.TryGetValue(playerIndex, out var instance) && instance != null && instance.State == SoundState.Playing)
            {
                instance.Stop();

            }

            CancelReloadInstance(playerIndex);
        }

        private static void CancelReloadInstance(int playerIndex)
        {
            if (_reloadInstances.TryGetValue(playerIndex, out var instance) && instance != null && instance.State == SoundState.Playing)
            {
                instance.Stop();
            }
        }

        public static void PushSounds(ISoundFrameContainer soundFrameContainer)
        {
            soundFrameContainer.SoundFrame = _soundFrame;
            _soundFrame = null;
        }

        public static bool HasAny() => _soundFrame != null;
    }
}
