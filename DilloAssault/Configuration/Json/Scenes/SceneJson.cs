using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Json.Scenes
{
    public class SceneJson
    {
        public HurtBoxListJson CollisionBoxes { get; set; }
        public List<TileListJson> TileLists { get; set; }
    }
}
