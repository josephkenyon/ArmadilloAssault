using DilloAssault.GameState.Battle.Crates;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Frame
{
    public class CrateFrame
    {
        public List<CrateType> Types { get; set; } = [];
        public List<float> PositionXs { get; set; } = [];
        public List<float> PositionYs { get; set; } = [];
        public List<bool> Groundeds { get; set; } = [];
    }
}
