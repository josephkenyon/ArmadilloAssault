using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Menu.Lobby
{
    public class LobbyState
    {
        private static IEnumerable<string> SelectableLevelKeys => ConfigurationManager.SceneConfigurations.Keys.Where(key => ConfigurationManager.SceneConfigurations[key].PreviewTexture != TextureName.nothing);

        public Dictionary<PlayerIndex, Avatar> Avatars { get; private set; } = [];
        public string SelectedLevel { get; private set; } = SelectableLevelKeys.First();
        public bool LevelSelect { get; private set; } = false;

        public void AvatarSelected(PlayerIndex index, AvatarType avatarType)
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
                    Avatars[index] = new Avatar(ConfigurationManager.GetAvatarConfiguration(avatarType));
                }
            }
            else
            {
                Avatars.Add(index, new Avatar(ConfigurationManager.GetAvatarConfiguration(avatarType)));
            }

            var rectangle = GetPlayerBackgroundRectangles()[index];

            SoundManager.QueueAvatarSound(avatarType, AvatarSound.Ready);

            Avatars[index].SetX(rectangle.X);
            Avatars[index].SetY(rectangle.Y + 16);
        }

        public static Dictionary<PlayerIndex, Rectangle> GetPlayerBackgroundRectangles()
        {
            var rectangleDictionary = new Dictionary<PlayerIndex, Rectangle>();

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

                rectangleDictionary.Add((PlayerIndex)playerIndex, new Rectangle(x, 448, 144, 256));
            }

            return rectangleDictionary;
        }

        public LobbyFrame CreateFrame()
        {
            var frame = new LobbyFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars),
                PlayerBackgrounds = GetPlayerBackgroundRectangles().Values.Select(RectangleJson.CreateFrom).ToList(),
                PlayerBackgroundIds = ServerManager.PlayerIndices,
                LevelSelect = LevelSelect,
                SelectedLevel = SelectedLevel,
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

        public void AvatarDisconnected(PlayerIndex index)
        {
            Avatars.Remove(index);
        }

        public void SetLevelSelect(bool levelSelect)
        {
            LevelSelect = levelSelect;
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
    }
}
