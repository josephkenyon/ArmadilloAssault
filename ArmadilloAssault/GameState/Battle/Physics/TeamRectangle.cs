using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Physics
{
    public class TeamRectangle(Rectangle rectangle, int teamIndex, bool returnZone = false, bool allowLeftEdge = false, bool allowRightEdge = false)
    {
        public int TeamIndex { get; private set; } = teamIndex;
        public readonly Rectangle Rectangle = rectangle;
        public readonly bool ReturnZone = returnZone;
        public readonly bool AllowLeftEdge = allowLeftEdge;
        public readonly bool AllowRightEdge = allowRightEdge;

        public void SetTeamIndex(int index)
        {
            TeamIndex = index;
        }
    }
}
