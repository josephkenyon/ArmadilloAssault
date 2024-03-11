using ArmadilloAssault.Configuration.Generics;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class StartingPositions
    {
        public PointJson First { get; set; } = PointJson.Zero;
        public PointJson Second { get; set; } = PointJson.Zero;
        public PointJson Third { get; set; } = PointJson.Zero;
        public PointJson Fourth { get; set; } = PointJson.Zero;
    }
}
