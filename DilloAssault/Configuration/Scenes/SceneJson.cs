using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Graphics.Drawing.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class SceneJson
    {
        public string BackgroundTexture { get; set; }
        public HurtBoxListJson CollisionBoxes { get; set; }
        public List<TileListJson> TileLists { get; set; }
    }
}
