using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.Sound;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class LobbyFrame : ISoundFrameContainer
    {
        public AvatarFrame AvatarFrame { get; set; } = new();
        public SoundFrame SoundFrame { get; set; } = new();
        public List<RectangleJson> PlayerBackgrounds { get; set; } = [];
        public List<int> PlayerBackgroundIds { get; set; } = [];
        public List<int> PlayerTeamIds { get; set; } = [];
        public List<string> PlayerNames { get; set; } = [];
        public bool LevelSelect { get; set; } = false;
        public bool ModeSelect { get; set; } = false;
        public int TileSize { get; set; } = 24;
        public string SelectedLevel { get; set; } = "";
        public Mode SelectedMode { get; set; } = Mode.Deathmatch;
        public string ModeName { get; set; } = "";
    }
}
