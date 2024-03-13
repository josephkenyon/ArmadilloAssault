﻿using ArmadilloAssault.Assets;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public class ModeManager
    {
        public readonly Mode Mode;

        private Dictionary<int, int> PlayerTeamRelations { get; set; } = [];
        private Dictionary<int, IndividualBattleStat> IndividualBattleStats { get; set; } = [];
        private Dictionary<int, TeamBattleStat> TeamBattleStats { get; set; } = [];
        private Dictionary<int, bool> Disconnecteds { get; set; } = [];

        private bool GameOverOverride { get; set; }

        public bool GameOver => GameOverOverride || IsGameOver();

        public bool TeamsEnabled => TeamBattleStats.Count != IndividualBattleStats.Count;

        public int RespawnFrames => 60 * (IndividualBattleStats.Count > 2 ? 10 : 5);

        public int GetTeamIndex(int playerIndex) => PlayerTeamRelations[playerIndex];

        private List<int> ContestingTeamIndicies { get; set; } = [];

        public int? CaputurePointSeconds => ContestingTeamIndicies.Count == 1 ? TeamBattleStats[ContestingTeamIndicies.First()].CapturePointFrames / 60 : null;

        public void UpdateKingOfTheHill(Rectangle capturePointBox, ICollection<Avatar> avatars)
        {
            ContestingTeamIndicies = avatars.Where(avatar => capturePointBox.Intersects(avatar.GetCollisionBox())).Select(avatar => PlayerTeamRelations[avatar.PlayerIndex]).Distinct().ToList();

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

        public ModeManager(IEnumerable<KeyValuePair<int, int>> playerIndices, Mode mode)
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
            if (Mode == Mode.Deathmatch)
            {
                if ((TeamsEnabled && TeamBattleStats.Values.Any(stat => stat.Kills >= 10)) || IndividualBattleStats.Values.Any(stat => stat.Kills >= 5))
                {
                    return true;
                }
            }
            else if (Mode == Mode.King_of_the_Hill)
            {
                if (TeamBattleStats.Values.Any(stat => (stat.CapturePointFrames / 60) >= 5))
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
