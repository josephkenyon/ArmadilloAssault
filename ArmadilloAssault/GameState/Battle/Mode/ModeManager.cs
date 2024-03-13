using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public class ModeManager
    {
        public Mode Mode { get; private set; }

        private Dictionary<int, BattleStat> BattleStats { get; set; } = [];
        private Dictionary<int, bool> Disconnecteds { get; set; } = [];

        private bool GameOverOverride { get; set; }

        public bool GameOver => GameOverOverride || IsGameOver();

        public int RespawnFrames => 60 * (BattleStats.Count > 2 ? 10 : 5);

        public ModeManager(IEnumerable<PlayerIndex> playerIndices)
        {
            Clear();

            foreach (var playerIndex in playerIndices)
            {
                BattleStats.Add((int)playerIndex, new BattleStat());
            }
        }

        public void Clear()
        {
            GameOverOverride = false;

            BattleStats.Clear();
            Disconnecteds.Clear();
        }

        public void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            BattleStats[hitIndex].DamageTaken += damage;
            BattleStats[firedIndex].DamageDealt += damage;
        }

        public void AvatarKilled(int deadIndex, int? killIndex)
        {
            BattleStats[deadIndex].Deaths += 1;
            if (killIndex != null)
            {
                BattleStats[(int)killIndex].Kills += 1;
            }
        }

        private bool IsGameOver()
        {
            if (Mode == Mode.Deathmatch)
            {
                if (BattleStats.Values.Any(stat => stat.Kills >= 2))
                {
                    return true;
                }
            }

            return false;
        }

        public void OverrideGameOver()
        {
            GameOverOverride = true;
        }
    }
}
