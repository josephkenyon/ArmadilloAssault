﻿using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Configuration.Generics
{
    public class ColorJson
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public ColorJson()
        {
            R = 100;
            G = 149;
            B = 237;
        }

        public ColorJson(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static ColorJson White => new() { R = 255, G = 255, B = 255 };

        public static ColorJson CreateFrom(Color color) => new() { R = color.R, G = color.G, B = color.B };

        public Color ToColor() => new(R, G, B);
    }
}
