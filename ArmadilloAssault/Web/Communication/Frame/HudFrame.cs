using ArmadilloAssault.Configuration.Generics;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class HudFrame
    {
        public List<int> PlayerIndices { get; set; } = [];
        public List<bool> Deads { get; set; } = [];
        public List<bool> Visibles { get; set; } = [];
        public List<int> AvatarXs { get; set; } = [];
        public List<int> AvatarYs { get; set; } = [];
        public List<int> Healths { get; set; } = [];
        public List<int?> Ammos { get; set; } = [];
        public ColorJson CapturePointColor { get; set; }
        public int? CapturePointSeconds { get; set; }
    }
}
