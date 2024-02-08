using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Generics
{
    public class PointJson
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point ToPoint() => new(X, Y);
    }
}
