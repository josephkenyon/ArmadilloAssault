using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Mode;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class HudFrame
    {
        public ModeType ModeType { get; set; }
        public List<int> TeamIndices { get; set; } = [];
        public List<int> ModeValues { get; set; } = [];
        public List<int> PlayerIndices { get; set; } = [];
        public List<bool> Deads { get; set; } = [];
        public List<bool> Visibles { get; set; } = [];
        public List<int> AvatarXs { get; set; } = [];
        public List<int> AvatarYs { get; set; } = [];
        public List<int> Healths { get; set; } = [];
        public List<int?> Ammos { get; set; } = [];
        public ColorJson CapturePointColor { get; set; }
        public int? CapturePointSeconds { get; set; }

        public List<int> FlagTimerValues { get; set; } = [];
        public List<float> FlagTimerXs { get; set; } = [];
        public List<float> FlagTimerYs { get; set; } = [];

    }
}
