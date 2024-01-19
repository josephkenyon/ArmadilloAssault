using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Assets
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
