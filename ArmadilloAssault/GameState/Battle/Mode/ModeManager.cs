using ArmadilloAssault.Assets;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Web.Communication.Update;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Mode
{
    public class ModeManager
    {
        public readonly IModeManagerListener ModeManagerListener;
        public readonly ModeType Mode;

        public Dictionary<int, int> PlayerTeamRelations { get; private set; } = [];
        private Dictionary<int, IndividualBattleStat> IndividualBattleStats { get; set; } = [];
        private Dictionary<int, TeamBattleStat> TeamBattleStats { get; set; } = [];
        private Dictionary<int, bool> Disconnecteds { get; set; } = [];
        public Dictionary<int, int> CrownedPlayerIndices { get; set; } = [];
        public Dictionary<int, Point> FlagStartingLocations { get; private set; } = [];
        public Dictionary<int, int> FlagReturnTimers { get; private set; } = [];

        public string VictoryMessage { get; private set; }

        public bool GameOver => IsGameOver();

        public bool TeamsEnabled => TeamBattleStats.Count != IndividualBattleStats.Count;

        public int RespawnFrames => 60 * (IndividualBattleStats.Count > 2 ? 10 : 5);

        public int GetTeamIndex(int playerIndex) => PlayerTeamRelations[playerIndex];

        private List<int> ContestingTeamIndicies { get; set; } = [];

        public int? CapturePointSeconds => ContestingTeamIndicies.Count == 1 ? TeamBattleStats[ContestingTeamIndicies.First()].CapturePointFrames / 60 : null;

        private bool newData = true;

        public ModeManager(IModeManagerListener modeManagerListener, IEnumerable<KeyValuePair<int, int>> playerIndices, ModeType mode, Dictionary<int, AvatarProp> avatarProps)
        {
            ModeManagerListener = modeManagerListener;

            foreach (var playerIndexPair in playerIndices)
            {
                PlayerTeamRelations.Add(playerIndexPair.Key, playerIndexPair.Value);
                TeamBattleStats.TryAdd(playerIndexPair.Value, new TeamBattleStat());
                IndividualBattleStats.Add(playerIndexPair.Key, new IndividualBattleStat());
            }

            Mode = mode;

            if (mode == ModeType.Regicide)
            {
                foreach (var avatar in avatarProps)
                {
                    var teamIndex = PlayerTeamRelations[avatar.Key];

                    if (avatar.Value.Crowned)
                    {
                        CrownedPlayerIndices.Add(teamIndex, avatar.Key);
                    }
                }
            }
        }

        public void InitializeCaptureTheFlag(Dictionary<int, Point> flags)
        {
            foreach (var flag in flags)
            {
                FlagStartingLocations.Add(flag.Key, new Point(flag.Value.X, flag.Value.Y));
                FlagReturnTimers.Add(flag.Key, 0);
            }
        }

        public void UpdateKingOfTheHill(Rectangle capturePointBox, ICollection<Avatar> avatars)
        {
            ContestingTeamIndicies = avatars.Where(avatar => !avatar.IsDead && capturePointBox.Intersects(avatar.GetCollisionBox())).Select(avatar => PlayerTeamRelations[avatar.PlayerIndex]).Distinct().ToList();

            if (ContestingTeamIndicies.Count == 1 && !GameOver)
            {
                var capturePointSeconds = CapturePointSeconds;

                TeamBattleStats[ContestingTeamIndicies.First()].CapturePointFrames++;

                newData = capturePointSeconds != CapturePointSeconds;
            }
        }

        public void UpdateCaptureTheFlag(IEnumerable<TeamRectangle> returnZones, IEnumerable<Item> flags)
        {
            foreach (var zone in returnZones)
            {
                var flag = flags.FirstOrDefault(item => item.GetCollisionBox().Intersects(zone.Rectangle) && zone.TeamIndex != item.TeamIndex);

                if (flag != null)
                {
                    Trace.WriteLine(zone.TeamIndex);
                    TeamBattleStats[zone.TeamIndex].Points++;
                    var avatar = ModeManagerListener.GetAvatars().Values.SingleOrDefault(avatar => avatar.HeldItems.Contains(flag));
                    avatar?.HeldItems.Remove(flag);

                    flag.SetPosition(FlagStartingLocations[flag.TeamIndex].ToVector2());
                    flag.Disturbed = false;

                    newData = true;
                }
            }

            foreach (var flag in flags)
            {
                if (flag.Disturbed && !flag.BeingHeld && flag.Position.ToPoint() != FlagStartingLocations[flag.TeamIndex])
                {
                    FlagReturnTimers[flag.TeamIndex]++;
                }
                else
                {
                    FlagReturnTimers[flag.TeamIndex] = 0;
                }
            }

            foreach (var timer in FlagReturnTimers)
            {
                if (timer.Value == 60 * 5)
                {
                    var flag = flags.Single(flag => flag.TeamIndex == timer.Key);
                    flag.SetPosition(FlagStartingLocations[flag.TeamIndex].ToVector2());
                    flag.Disturbed = false;
                }
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

        public void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            IndividualBattleStats[hitIndex].DamageTaken += damage;
            IndividualBattleStats[firedIndex].DamageDealt += damage;

            newData = true;
        }

        public void AvatarKilled(int deadIndex, int? killIndex)
        {
            IndividualBattleStats[deadIndex].Deaths += 1;
            if (killIndex != null)
            {
                IndividualBattleStats[(int)killIndex].Kills += 1;
                TeamBattleStats[PlayerTeamRelations[(int)killIndex]].Kills += 1;
            }

            newData = true;
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
            else if (Mode == ModeType.Capture_the_Flag)
            {
                var teamIndex = TeamBattleStats.Keys.SingleOrDefault(index => TeamBattleStats[index].Points >= 3, -1);
                if (teamIndex != -1)
                {
                    VictoryMessage = $"Team  {GetTeamString(teamIndex)}  Wins!";
                    return true;
                }
            }
            else if (Mode == ModeType.Regicide)
            {
                var avatars = ModeManagerListener.GetAvatars();
                var aliveIndices = CrownedPlayerIndices.Where(playerIndex => !avatars[playerIndex.Value].IsDead);

                if (aliveIndices.Count() == 1)
                {
                    VictoryMessage = $"Team  {GetTeamString(aliveIndices.First().Key)}  Wins!";
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

        public List<int> GetModeValues()
        {
            if (Mode == ModeType.King_of_the_Hill)
            {
                return TeamBattleStats.Select(stat => stat.Value.CapturePointFrames / 60).ToList();
            }
            else if (Mode == ModeType.Capture_the_Flag)
            {
                return TeamBattleStats.Select(stat => stat.Value.Points).ToList();
            }
            else
            {
                return TeamBattleStats.Select(stat => stat.Value.Kills).ToList();
            }
        }

        public bool GetPlayerShouldRespawn(int playerIndex)
        {
            if (GameOver)
            {
                return false;
            }

            if (Mode == ModeType.Regicide)
            {
                var teamIndex = GetTeamIndex(playerIndex);

                var crownedPlayerIndex = CrownedPlayerIndices[teamIndex];
                if (playerIndex == crownedPlayerIndex)
                {
                    return false;
                }
                else
                {
                    return !ModeManagerListener.GetAvatars()[crownedPlayerIndex].IsDead;
                }
            }

            return true;
        }

        public StatFrame CreateStatFrameIfNewData()
        {
            if (!newData)
            {
                return null;
            }

            var statFrame = new StatFrame();

            foreach (var playerIndex in PlayerTeamRelations.Keys)
            {
                statFrame.PlayerIndices.Add(playerIndex);
                statFrame.Names.Add(ServerManager.GetPlayerName(playerIndex));

                var playerStat = IndividualBattleStats[playerIndex];

                if (playerStat != null)
                {
                    statFrame.Kills.Add(playerStat.Kills);
                    statFrame.Deaths.Add(playerStat.Deaths);
                    statFrame.DamageDealts.Add(playerStat.DamageDealt);
                    statFrame.DamageTakens.Add(playerStat.DamageTaken);
                }
            }

            newData = false;

            return statFrame;
        }
    }
}
