using DilloAssault.Configuration.Effects;
using DilloAssault.Generics;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public class EffectsUpdate : BaseUpdate
    {
        public List<EffectType> Types { get; set; } = [];
        public List<Direction> Directions { get; set; } = [];
        public List<int> Frames { get; set; } = [];
    }
}
