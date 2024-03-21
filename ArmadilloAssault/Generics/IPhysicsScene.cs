using ArmadilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.Generics
{
    public interface IPhysicsScene
    {
        ICollection<Rectangle> GetCollisionBoxes();
        ICollection<TeamRectangle> GetTeamRectangles();
        Point GetSize();
        bool YWraps();
    }
}
