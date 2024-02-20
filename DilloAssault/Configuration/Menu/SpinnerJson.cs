using DilloAssault.Configuration.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Menu
{
    public class SpinnerJson
    {
        public string Title { get; set; }
        public MenuAction Action { get; set; }
        public List<string> Data { get; set; }
        public PointJson Position { get; set; }
        public PointJson Size { get; set; }

        public Rectangle GetRectangle() => new(Position.ToPoint(), Size.ToPoint());
    }
}
