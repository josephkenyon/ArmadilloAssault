using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Sound;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class SoundFrame
    {
        [JsonProperty("ASs")]
        public List<KeyValuePair<AvatarType, AvatarSound>> AvatarSounds { get; set; } = [];

        [JsonProperty("BSs")]
        public List<BattleSound> BattleSounds { get; set; } = [];

        [JsonProperty("WSs")]
        public List<WeaponType> WeaponSounds { get; set; } = [];

        [JsonProperty("P")]
        public bool Played { get; set; } = false;

        [JsonProperty("CRS")]
        public bool CancelReloudSound { get; set; } = false;
    }
}
