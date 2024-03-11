using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class SceneJson
    {
        public TextureName BackgroundTexture { get; set; } = TextureName.mountain_background;
        public TextureName TilesetTexture { get; set; } = TextureName.test_tileset;
        public PointJson Size { get; set; } = new PointJson(1920, 1080);
        public StartingPositions StartingPositions { get; set; } = new();
        public ColorJson BackgroundColor { get; set; } = new();
        public bool HighCloudsOnly { get; set; }
        public FlowJson Flow { get; set; }
        public EnvironmentalEffectJson EnvironmentalEffects { get; set; }
        public RectangleListJson CollisionBoxes { get; set; } = new();
        public List<TileListJson> TileLists { get; set; } = [];
    }
}
