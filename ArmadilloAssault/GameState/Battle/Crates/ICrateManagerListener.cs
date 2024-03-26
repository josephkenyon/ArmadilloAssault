using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Crates
{
    public interface ICrateManagerListener
    {
        ICollection<Rectangle> GetCollisionBoxes();
        Point GetSceneSize();
        void CrateCreated(Crate crate);
        void CrateDeleted(int id);
    }
}
