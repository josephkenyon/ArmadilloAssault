﻿using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Environment.Precipitation;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Scenes
{
    public class SceneJson
    {
        public TextureName BackgroundTexture { get; set; } = TextureName.mountain_background;
        public TextureName TilesetTexture { get; set; } = TextureName.test_tileset;
        public bool WrapY { get; set; } = true;
        public PointJson Size { get; set; } = new PointJson(1920, 1080);
        public List<PointJson> StartingPositions { get; set; } = [];
        public ColorJson BackgroundColor { get; set; } = new();
        public bool HighCloudsOnly { get; set; }
        public FlowJson Flow { get; set; }
        public PrecipitationType PrecipitationType { get; set; }
        public EnvironmentalEffectJson EnvironmentalEffects { get; set; }
        public RectangleListJson CollisionBoxes { get; set; } = new();
        public RectangleJson CapturePoint { get; set; }
        public List<TileListJson> TileLists { get; set; } = [];
    }
}
