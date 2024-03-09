using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class SceneJson
    {
        public TextureName BackgroundTexture { get; set; }
        public TextureName PreviewTexture { get; set; }
        public TextureName TilesetTexture { get; set; }
        public PointJson Size { get; set; }
        public StartingPositions StartingPositions { get; set; }
        public ColorJson BackgroundColor { get; set; }
        public bool HighCloudsOnly { get; set; }
        public FlowJson Flow { get; set; }
        public EnvironmentalEffectJson EnvironmentalEffects { get; set; }
        public HurtBoxListJson CollisionBoxes { get; set; }
        public List<TileListJson> TileLists { get; set; }
    }
}
