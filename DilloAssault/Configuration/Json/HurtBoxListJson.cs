using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Json
{
    public class HurtBoxListJson
    {
        public List<int> X { get; set; } = [];
        public List<int> Y { get; set; } = [];
        public List<int> Width { get; set; } = [];
        public List<int> Height { get; set; } = [];
    }
}
