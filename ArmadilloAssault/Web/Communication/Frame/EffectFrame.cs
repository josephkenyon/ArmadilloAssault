using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class EffectFrame
    {
        public List<EffectType> Types { get; set; } = [];
        public List<float> PositionXs { get; set; } = [];
        public List<float> PositionYs { get; set; } = [];
        public List<Direction?> Directions { get; set; } = [];
        public List<int> Frames { get; set; } = [];
    }
}
