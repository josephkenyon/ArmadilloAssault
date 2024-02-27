using ArmadilloAssault.Configuration.Generics;
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
        public List<string> PlayerNames { get; set; } = [];
        public bool LevelSelect { get; set; } = false;
        public string SelectedLevel { get; set; } = "";
    }
}
