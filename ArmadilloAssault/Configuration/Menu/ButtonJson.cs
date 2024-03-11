using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Menus
{
    public class ButtonJson
    {
        public string Text { get; set; }
        public MenuKey TextKey { get; set; }

        public bool Unselectable { get; set; }

        public TextureName TextureName { get; set; }
        public MenuKey TextureKey { get; set; }

        public PointJson Location { get; set; }
        public PointJson Size { get; set; }

        public MenuCondition EnabledCondition { get; set; }

        public List<MenuCondition> Conditions { get; set; } = [];
        public List<MenuAction> Actions { get; set; } = [];

        public string Data { get; set; }
    }
}
