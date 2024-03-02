using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public static class ModeManager
    {
        public static Mode Mode { get; private set; }

        private static Dictionary<int, BattleStat> BattleStats { get; set; } = [];

        private static bool GameOver => IsGameOver();

        public static void Initialize(IEnumerable<PlayerIndex> playerIndices)
        {
            BattleStats.Clear();

            foreach (var playerIndex in playerIndices)
            {
                BattleStats.Add((int)playerIndex, new BattleStat());
            }
        }

        public static void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            BattleStats[hitIndex].DamageTaken += damage;
            BattleStats[firedIndex].DamageDealt += damage;
        }

        public static void AvatarKilled(int deadIndex, int killIndex)
        {
            BattleStats[deadIndex].Deaths += 1;
            BattleStats[killIndex].Kills += 1;

            BattleManager.SetRespawnTimer(deadIndex, (60 * (BattleStats.Count > 2 ? 8 : 4)) + 59);
        }

        private static bool IsGameOver()
        {
            if (Mode == Mode.Death_Match)
            {
                if (BattleStats.Values.Any(stat => stat.Kills >= 5))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
