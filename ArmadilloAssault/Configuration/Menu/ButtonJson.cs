using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Menu
{
    public class ButtonJson
    {
        public string Text { get; set; }
        public MenuCondition Condition { get; set; }
        public MenuCondition EnabledCondition { get; set; }
        public TextureName TextureName { get; set; }
        public PointJson Location { get; set; }
        public PointJson Size { get; set; }
        public List<MenuAction> Actions { get; set; }
        public string Data { get; set; }
    }
}
