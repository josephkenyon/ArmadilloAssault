using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Sound;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class SoundFrame
    {
        [JsonProperty("A", NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<AvatarType, AvatarSound>> AvatarSounds { get; set; }

        [JsonProperty("B", NullValueHandling = NullValueHandling.Ignore)]
        public List<BattleSound> BattleSounds { get; set; }

        [JsonProperty("W", NullValueHandling = NullValueHandling.Ignore)]
        public List<WeaponType> WeaponSounds { get; set; }

        [JsonProperty("R", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> PlayerReloads { get; set; }

        [JsonProperty("E", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> PlayerReloadEnds { get; set; }

        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> CancelReloudSounds { get; set; }
    }
}
