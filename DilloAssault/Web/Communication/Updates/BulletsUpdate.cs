using DilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public class BulletsUpdate : BaseUpdate
    {
        public List<TextureName> Textures { get; set; } = [];
        public List<float> Rotations { get; set; } = [];
        public List<int> SizeXs { get; set; } = [];
        public List<int> SizeYs { get; set; } = [];
    }
}
