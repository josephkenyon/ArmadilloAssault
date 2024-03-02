using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public static class ModeManager
    {
        public static Mode Mode { get; private set; }

        private static Dictionary<int, BattleStat> BattleStats { get; set; } = [];
        private static Dictionary<int, bool> Disconnecteds { get; set; } = [];

        private static bool GameOverOverride { get; set; }

        public static bool GameOver => GameOverOverride || IsGameOver();

        public static void Initialize(IEnumerable<PlayerIndex> playerIndices)
        {
            Clear();

            foreach (var playerIndex in playerIndices)
            {
                BattleStats.Add((int)playerIndex, new BattleStat());
            }
        }

        public static void Clear()
        {
            GameOverOverride = false;

            BattleStats.Clear();
            Disconnecteds.Clear();
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

            if (!GameOver)
            {
                BattleManager.SetRespawnTimer(deadIndex, (60 * (BattleStats.Count > 2 ? 10 : 5)));
            }
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

        public static void OverrideGameOver()
        {
            GameOverOverride = true;
        }
    }
}
