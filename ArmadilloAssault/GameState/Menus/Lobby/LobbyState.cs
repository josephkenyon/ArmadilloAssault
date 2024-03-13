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
using System.Reflection;

namespace ArmadilloAssault.GameState.Menus.Lobby
{
    public class LobbyState
    {
        private static IEnumerable<string> SelectableLevelKeys => ConfigurationManager.SceneConfigurations.Keys;

        public Dictionary<int, Avatar> Avatars { get; private set; } = [];
        public Dictionary<int, int> PlayerTeamRelations { get; private set; } = [];
        public string SelectedLevel { get; private set; } = SelectableLevelKeys.First();
        public Mode SelectedMode { get; private set; } = Mode.Deathmatch;
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

            if (Avatars.Values.Any(avatar => avatar.Type == avatarType)) return;

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

        public static Dictionary<int, Rectangle> GetPlayerBackgroundRectangles()
        {
            var rectangleDictionary = new Dictionary<int, Rectangle>();

            foreach (var playerIndex in ServerManager.PlayerIndices)
            {
                var middle = 1920 / 2;
                var x = middle;
                if (playerIndex == 0)
                {
                    x -= 288 + 96;
                }
                else if (playerIndex == 1)
                {
                    x -= 144 + 32;
                }
                else if (playerIndex == 2)
                {
                    x += 32;
                }
                else if (playerIndex == 3)
                {
                    x += 144 + 96;
                }

                rectangleDictionary.Add(playerIndex, new Rectangle(x, 448, 144, 256));
            }

            return rectangleDictionary;
        }

        public LobbyFrame CreateFrame()
        {
            var frame = new LobbyFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars),
                PlayerBackgrounds = GetPlayerBackgroundRectangles().Values.Select(RectangleJson.CreateFrom).ToList(),
                PlayerBackgroundIds = [.. PlayerTeamRelations.Keys],
                PlayerTeamIds = [.. PlayerTeamRelations.Values],
                LevelSelect = LevelSelect,
                ModeSelect = ModeSelect,
                SelectedLevel = SelectedLevel,
                SelectedMode = SelectedMode,
                ModeName = SelectedMode.ToString().Replace("_", " "),
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
            var values = Enum.GetValues<Mode>().ToList();

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
            var values = Enum.GetValues<Mode>().ToList();

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

            PlayerTeamRelations[playerIndex] = newTeamIndex;
        }
    }
}
