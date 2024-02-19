using DilloAssault.Configuration.Weapons;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Frame
{
    public class BulletFrame
    {
        public List<WeaponType> WeaponTypes { get; set; } = [];
        public List<float> Rotations { get; set; } = [];
        public List<float> PositionXs { get; set; } = [];
        public List<float> PositionYs { get; set; } = [];
    }
}
