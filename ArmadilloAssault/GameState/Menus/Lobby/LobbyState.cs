using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Menus.Lobby
{
    public class LobbyState
    {
        private static IEnumerable<string> SelectableLevelKeys => ConfigurationManager.SceneConfigurations.Keys;

        public Dictionary<int, Avatar> Avatars { get; private set; } = [];
        public Dictionary<int, int> PlayerTeamRelations { get; private set; } = [];
        public string SelectedLevel { get; private set; } = SelectableLevelKeys.First();
        public ModeType SelectedMode { get; private set; } = ModeType.Deathmatch;
        public bool LevelSelect { get; private set; } = false;
        public bool ModeSelect { get; private set; } = false;

        public LobbyState()
        {
            PlayerTeamRelations.TryAdd(0, 0);
        }

        public void AvatarSelected(int index, AvatarType avatarType)
        {
            if (Avatars.TryGetValue(index, out Avatar value) && value.Type == avatarType)
            {
                Avatars.Remove(index);
                return;
            }

            if (Avatars.Any(avatar => avatar.Value.Type == avatarType && PlayerTeamRelations[avatar.Key] == PlayerTeamRelations[index])) return;

            if (Avatars.TryGetValue(index, out value))
            {
                if (value.Type != avatarType)
                {
                    Avatars[index] = new Avatar(index, ConfigurationManager.GetAvatarConfiguration(avatarType));
                }
            }
            else
            {
                Avatars.Add(index, new Avatar(index, ConfigurationManager.GetAvatarConfiguration(avatarType)));
            }

            var rectangle = GetPlayerBackgroundRectangles()[index];

            SoundManager.QueueAvatarSound(avatarType, AvatarSound.Ready);

            Avatars[index].SetX(rectangle.X);
            Avatars[index].SetY(rectangle.Y + 16);
        }

        public Dictionary<int, Rectangle> GetPlayerBackgroundRectangles()
        {
            var offset = ModeSelect ? 32 : 0;

            var rectangleDictionary = new Dictionary<int, Rectangle>();

            foreach (var playerIndex in ServerManager.PlayerIndices)
            {
                var middle = 1920 / 2;
                var x = middle;
                if (playerIndex == 0)
                {
                    x -= 432 + 160;
                }
                if (playerIndex == 1)
                {
                    x -= 288 + 96;
                }
                else if (playerIndex == 2)
                {
                    x -= 144 + 32;
                }
                else if (playerIndex == 3)
                {
                    x += 32;
                }
                else if (playerIndex == 4)
                {
                    x += 144 + 96;
                }
                else if (playerIndex == 5)
                {
                    x += 288 + 160;
                }

                rectangleDictionary.Add(playerIndex, new Rectangle(x, 448 + offset, 144, 256));
            }

            return rectangleDictionary;
        }

        public Dictionary<int, Rectangle> GetPlayerModeButtonRectangles()
        {
            var rectangleDictionary = new Dictionary<int, Rectangle>();

            foreach (var playerIndex in ServerManager.PlayerIndices)
            {
                var middle = 1920 / 2;
                var x = middle;
                if (playerIndex == 0)
                {
                    x -= 432 + 160;
                }
                if (playerIndex == 1)
                {
                    x -= 288 + 96;
                }
                else if (playerIndex == 2)
                {
                    x -= 144 + 32;
                }
                else if (playerIndex == 3)
                {
                    x += 32;
                }
                else if (playerIndex == 4)
                {
                    x += 144 + 96;
                }
                else if (playerIndex == 5)
                {
                    x += 288 + 160;
                }

                rectangleDictionary.Add(playerIndex, new Rectangle(x, 448 + 32 + 256 + 16, 144, 64));
            }

            return rectangleDictionary;
        }

        public LobbyFrame CreateFrame()
        {
            var frame = new LobbyFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars, PlayerTeamRelations, ModeSelect ? 32 : 0, ModeSelect && ModeType.Regicide == SelectedMode),
                PlayerBackgrounds = GetPlayerBackgroundRectangles().Values.Select(background => RectangleJson.CreateFrom(background)).ToList(),
                PlayerModeButtons = GetPlayerModeButtonRectangles().Values.Select(background => RectangleJson.CreateFrom(background)).ToList(),
                PlayerBackgroundIds = [.. PlayerTeamRelations.Keys],
                PlayerTeamIds = [.. PlayerTeamRelations.Values],
                LevelSelect = LevelSelect,
                ModeSelect = ModeSelect,
                SelectedLevel = SelectedLevel,
                SelectedMode = SelectedMode,
                ModeName = SelectedMode.ToString().Replace("_", "  "),
                TileSize = GetTileSize()
            };

            SoundManager.PushSounds(frame);

            return frame;
        }

        public void Update()
        {
            foreach (var avatar in Avatars.Values)
            {
                avatar.Update();
            }
        }

        private int GetTileSize()
        {
            var sceneJson = ConfigurationManager.GetSceneConfiguration(SelectedLevel);
            var ratio = sceneJson.Size.X / (float)sceneJson.Size.Y;

            float tileSize = 48 * (1920f / (sceneJson.Size.X * 2));
            if (ratio > 1.8f)
            {
                tileSize = 48f * (1920f / sceneJson.Size.X);
            }

            else if (ratio < 1.76f)
            {
                tileSize = 48f * (1080f / sceneJson.Size.Y);
            }

            return (int)tileSize;
        }

        public void AvatarDisconnected(int index)
        {
            Avatars.Remove(index);
            PlayerTeamRelations.Remove(index);
        }

        public void SetLevelSelect(bool levelSelect)
        {
            LevelSelect = levelSelect;
        }

        public void SetModeSelect(bool modeSelect)
        {
            ModeSelect = modeSelect;
        }

        public void NextLevel()
        {
            var keys = SelectableLevelKeys.ToList();
            var index = keys.IndexOf(SelectedLevel) + 1;

            if (index == keys.Count)
            {
                index = 0;
            }

            SelectedLevel = keys[index];
        }

        public void PreviousLevel()
        {
            var keys = SelectableLevelKeys.ToList();
            var index = keys.IndexOf(SelectedLevel) - 1;

            if (index == -1)
            {
                index = keys.Count - 1;
            }

            SelectedLevel = keys[index];
        }

        public void NextMode()
        {
            var values = Enum.GetValues<ModeType>().Where(mode => mode != ModeType.Tutorial).ToList();

            var index = values.IndexOf(SelectedMode);

            index++;

            if (index == values.Count)
            {
                index = 0;
            }

            SelectedMode = values[index];
        }

        public void PreviousMode()
        {
            var values = Enum.GetValues<ModeType>().Where(mode => mode != ModeType.Tutorial).ToList();

            var index = values.IndexOf(SelectedMode);

            index--;

            if (index == -1)
            {
                index = values.Count - 1;
            }

            SelectedMode = values[index];
        }

        public void IncrementTeamIndex(int playerIndex)
        {
            var newTeamIndex = PlayerTeamRelations[playerIndex];
            newTeamIndex++;
            if (newTeamIndex > 5)
            {
                newTeamIndex = 0;
            }

            if (Avatars[playerIndex] != null)
            {
                var avatarType = Avatars[playerIndex].Type;

                while (Avatars.Any(avatar => avatar.Value.Type == avatarType && PlayerTeamRelations[avatar.Key] == newTeamIndex))
                {
                    newTeamIndex++;
                    if (newTeamIndex > 5)
                    {
                        newTeamIndex = 0;
                    }
                }
            }

            var avatar = Avatars[playerIndex];
            if (avatar != null)
            {
                avatar.Crowned = false;
            }

            PlayerTeamRelations[playerIndex] = newTeamIndex;
        }

        public void CrownPlayer(int crownedPlayerIndex)
        {
            var teamIndex = PlayerTeamRelations[crownedPlayerIndex];
            var playerIndices = PlayerTeamRelations.Where(relation => relation.Value == teamIndex).Select(relation => relation.Key).ToList();

            foreach (var playerIndex in playerIndices)
            {
                if (Avatars[playerIndex] != null)
                {
                    Avatars[playerIndex].Crowned = playerIndex == crownedPlayerIndex;
                }
            }
        }

        public bool ModeConditionsComplete()
        {
            if (ModeType.Regicide == SelectedMode)
            {
                var teamsCount = PlayerTeamRelations.Select(relation => relation.Value).Distinct().Count();

                return Avatars.Where(avatar => avatar.Value.Crowned).Count() == teamsCount;
            }

            return true;
        }
    }
}
