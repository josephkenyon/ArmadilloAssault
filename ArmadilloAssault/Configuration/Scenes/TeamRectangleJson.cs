using ArmadilloAssault.Configuration.Generics;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class TeamRectangleJson : RectangleJson
    {
        public int TeamIndex { get; set; }
        public bool ReturnZone { get; set; }
        public bool AllowLeftEdge { get; set; }
        public bool AllowRightEdge { get; set; }
    }
}
