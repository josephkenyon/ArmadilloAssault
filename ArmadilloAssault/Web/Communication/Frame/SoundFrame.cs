using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Sound;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class SoundFrame
    {
        public List<KeyValuePair<AvatarType, AvatarSound>> AvatarSounds { get; set; } = [];
        public List<BattleSound> BattleSounds { get; set; } = [];
        public List<WeaponType> WeaponSounds { get; set; } = [];
        public bool Played { get; set; } = false;
        public bool CancelReloadSound { get; set; } = false;
    }
}
