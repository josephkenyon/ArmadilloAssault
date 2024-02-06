using DilloAssault.Configuration.Generics;
using DilloAssault.Graphics.Drawing.Textures;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Scenes
{
    public class SceneJson
    {
        public string BackgroundTexture { get; set; }
        public HurtBoxListJson CollisionBoxes { get; set; }
        public List<TileListJson> TileLists { get; set; }
    }
}
