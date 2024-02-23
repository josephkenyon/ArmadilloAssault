using System.Collections.Generic;

namespace ArmadilloAssault.Assets
{
    public class TileList
    {
        public int Z { get; set; }
        public List<Tile> Tiles { get; set; }

        public TileList() {
            Tiles = [];
        }
    }
}
