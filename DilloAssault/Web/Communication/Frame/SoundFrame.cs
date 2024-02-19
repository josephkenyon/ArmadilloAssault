using DilloAssault.Configuration.Avatars;
using DilloAssault.Configuration.Weapons;
using DilloAssault.Sound;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Frame
{
    public class SoundFrame
    {
        public List<KeyValuePair<AvatarType, AvatarSound>> AvatarSounds { get; set; } = [];
        public List<BattleSound> BattleSounds { get; set; } = [];
        public List<WeaponType> WeaponSounds { get; set; } = [];
        public bool CancelReloadSound { get; set; } = false;
    }
}
