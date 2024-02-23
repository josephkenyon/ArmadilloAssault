using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Generics;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class EnvironmentalEffectJson
    {
        public EffectType EffectType { get; set; }
        public int StartingCount { get; set; }
        public int SpawnRate { get; set; }
        public RangeJson YRange { get; set; }
    }
}
