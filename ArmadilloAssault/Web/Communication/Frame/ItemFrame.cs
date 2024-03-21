using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Generics;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class ItemFrame
    {
        public List<ItemType> ItemTypes { get; set; } = [];
        public List<Direction> Directions { get; set; } = [];
        public List<int> SpriteXs { get; set; } = [];
        public List<int?> TeamIndices { get; set; } = [];
        public List<float> PositionXs { get; set; } = [];
        public List<float> PositionYs { get; set; } = [];
    }
}
