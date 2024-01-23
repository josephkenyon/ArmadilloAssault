using System.Collections.Generic;

namespace DilloAssault.Configuration.Json.Scenes
{
    public class TileListJson
    {
        public int Z { get; set; }
        public List<int> X { get; set; }
        public List<int> Y { get; set; }
        public List<int> SpriteX { get; set; }
        public List<int> SpriteY { get; set; }
        public string Texture { get; set; }
    }
}
