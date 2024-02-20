using ArmadilloAssault.GameState.Battle.Crates;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class CrateFrame
    {
        public List<CrateType> Types { get; set; } = [];
        public List<float> PositionXs { get; set; } = [];
        public List<float> PositionYs { get; set; } = [];
        public List<bool> Groundeds { get; set; } = [];
    }
}
