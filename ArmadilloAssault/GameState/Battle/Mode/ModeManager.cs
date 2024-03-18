using ArmadilloAssault.Assets;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public class ModeManager
    {
        public readonly ModeType Mode;

        public Dictionary<int, int> PlayerTeamRelations { get; private set; } = [];
        private Dictionary<int, IndividualBattleStat> IndividualBattleStats { get; set; } = [];
        private Dictionary<int, TeamBattleStat> TeamBattleStats { get; set; } = [];
        private Dictionary<int, bool> Disconnecteds { get; set; } = [];

        public string VictoryMessage { get; private set; }

        private bool GameOverOverride { get; set; }

        public bool GameOver => GameOverOverride || IsGameOver();

        public bool TeamsEnabled => TeamBattleStats.Count != IndividualBattleStats.Count;

        public int RespawnFrames => 60 * (IndividualBattleStats.Count > 2 ? 10 : 5);

        public int GetTeamIndex(int playerIndex) => PlayerTeamRelations[playerIndex];

        private List<int> ContestingTeamIndicies { get; set; } = [];

        public int? CaputurePointSeconds => ContestingTeamIndicies.Count == 1 ? TeamBattleStats[ContestingTeamIndicies.First()].CapturePointFrames / 60 : null;

        public void UpdateKingOfTheHill(Rectangle capturePointBox, ICollection<Avatar> avatars)
        {
            ContestingTeamIndicies = avatars.Where(avatar => !avatar.IsDead && capturePointBox.Intersects(avatar.GetCollisionBox())).Select(avatar => PlayerTeamRelations[avatar.PlayerIndex]).Distinct().ToList();

            if (ContestingTeamIndicies.Count == 1 && !GameOver)
            {
                TeamBattleStats[ContestingTeamIndicies.First()].CapturePointFrames++;
            }
        }

        public Color GetCapturePointColor()
        {
            var returnColor = Color.White;

            if (ContestingTeamIndicies.Count > 0)
            {
                returnColor = Color.Black;
            }

            ContestingTeamIndicies.ForEach(teamIndex =>
            {
                var teamColor = DrawingHelper.GetTeamColor(teamIndex).ToVector3();

                var color = returnColor.ToVector3();

                returnColor = new Color(
                    color.X + (teamColor.X / ContestingTeamIndicies.Count),
                    color.Y + (teamColor.Y / ContestingTeamIndicies.Count),
                    color.Z + (teamColor.Z / ContestingTeamIndicies.Count)
                );
            });

            return returnColor;
        }

        public ModeManager(IEnumerable<KeyValuePair<int, int>> playerIndices, ModeType mode)
        {
            foreach (var playerIndexPair in playerIndices)
            {
                PlayerTeamRelations.Add(playerIndexPair.Key, playerIndexPair.Value);
                TeamBattleStats.TryAdd(playerIndexPair.Value, new TeamBattleStat());
                IndividualBattleStats.Add(playerIndexPair.Key, new IndividualBattleStat());
            }

            Mode = mode;
        }

        public void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            IndividualBattleStats[hitIndex].DamageTaken += damage;
            IndividualBattleStats[firedIndex].DamageDealt += damage;
        }

        public void AvatarKilled(int deadIndex, int? killIndex)
        {
            IndividualBattleStats[deadIndex].Deaths += 1;
            if (killIndex != null)
            {
                IndividualBattleStats[(int)killIndex].Kills += 1;
                TeamBattleStats[PlayerTeamRelations[(int)killIndex]].Kills += 1;
            }
        }

        private bool IsGameOver()
        {
            if (Mode == ModeType.Deathmatch)
            {
                if (TeamsEnabled)
                {
                    var teamIndex = TeamBattleStats.Keys.SingleOrDefault(index => TeamBattleStats[index].Kills >= 10, -1);
                    if (teamIndex != -1)
                    {
                        VictoryMessage = $"Team  {GetTeamString(teamIndex)}  Wins!";
                        return true;
                    }
                }
                else
                {
                    var playerIndex = IndividualBattleStats.Keys.SingleOrDefault(index => IndividualBattleStats[index].Kills >= 5, -1);
                    if (playerIndex != -1)
                    {
                        VictoryMessage = $"Player  {playerIndex + 1}  Wins!";
                        return true;
                    }
                }
            }
            else if (Mode == ModeType.King_of_the_Hill)
            {
                var teamIndex = TeamBattleStats.Keys.SingleOrDefault(index => (TeamBattleStats[index].CapturePointFrames / 60) >= 20, -1);
                if (teamIndex != -1)
                {
                    VictoryMessage = $"Team  {GetTeamString(teamIndex)}  Wins!";
                    return true;
                }
            }

            return false;
        }

        private static string GetTeamString(int team)
        {
            return team switch
            {
                1 => "Red",
                2 => "Green",
                3 => "Yellow",
                4 => "Teal",
                5 => "Pink",
                _ => "Blue",
            };
        }

        public void OverrideGameOver()
        {
            GameOverOverride = true;
        }

        public List<int> GetModeValues()
        {
            var battleStats = TeamBattleStats.OrderBy(stat => stat.Key);

            if (Mode == ModeType.King_of_the_Hill)
            {
                return battleStats.Select(stat => stat.Value.CapturePointFrames / 60).ToList();
            }
            else
            {
                return battleStats.Select(stat => stat.Value.Kills).ToList();
            }
        }
    }
}
