using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Physics
{
    public class TeamRectangle(Rectangle rectangle, int teamIndex)
    {
        public readonly int TeamIndex = teamIndex;
        public readonly Rectangle Rectangle = rectangle;
    }
}
