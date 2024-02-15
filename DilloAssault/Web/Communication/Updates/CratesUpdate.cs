using DilloAssault.GameState.Battle.Crates;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public class CratesUpdate : BaseUpdate
    {
        public List<CrateType> Types { get; set; } = [];
        public List<bool> Groundeds { get; set; } = [];
    }
}
