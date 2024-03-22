using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.Sound;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class LobbyFrame : ISoundFrameContainer
    {
        [JsonProperty("LAF")]
        public LobbyAvatarFrame LobbyAvatarFrame { get; set; } = new();

        [JsonProperty("SF")]
        public SoundFrame SoundFrame { get; set; } = new();

        [JsonProperty("PBs")]
        public List<RectangleJson> PlayerBackgrounds { get; set; } = [];

        [JsonProperty("PMBs")]
        public List<RectangleJson> PlayerModeButtons { get; set; } = [];

        [JsonProperty("PBIs")]
        public List<int> PlayerBackgroundIds { get; set; } = [];

        [JsonProperty("PTIs")]
        public List<int> PlayerTeamIds { get; set; } = [];

        [JsonProperty("PNs")]
        public List<string> PlayerNames { get; set; } = [];

        [JsonProperty("LS")]
        public bool LevelSelect { get; set; } = false;

        [JsonProperty("MS")]
        public bool ModeSelect { get; set; } = false;

        [JsonProperty("TS")]
        public int TileSize { get; set; } = 24;

        [JsonProperty("SL")]
        public string SelectedLevel { get; set; } = "";

        [JsonProperty("SM")]
        public ModeType SelectedMode { get; set; } = ModeType.Deathmatch;

        [JsonProperty("MN")]
        public string ModeName { get; set; } = "";
    }
}
