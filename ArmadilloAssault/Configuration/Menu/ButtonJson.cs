using ArmadilloAssault.Configuration.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Menu
{
    public class ButtonJson
    {
        public string Text { get; set; }
        public List<MenuAction> Actions { get; set; }
        public string Data { get; set; }
        public PointJson Position { get; set; }
        public PointJson Size { get; set; }

        public Rectangle GetRectangle() => new(Position.ToPoint(), Size.ToPoint());
    }
}
