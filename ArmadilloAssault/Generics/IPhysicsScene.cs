using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.Generics
{
    public interface IPhysicsScene
    {
        ICollection<Rectangle> GetCollisionBoxes();
        Point GetSize();
        bool YWraps();
    }
}
