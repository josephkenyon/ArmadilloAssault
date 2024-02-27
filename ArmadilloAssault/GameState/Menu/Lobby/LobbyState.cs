using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
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
        public Dictionary<PlayerIndex, Avatar> Avatars { get; private set; } = [];

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

            var rectangle = GetPlayerBackgroundRectangles().ElementAt((int)index);

            Avatars[index].SetX(rectangle.X);
            Avatars[index].SetY(rectangle.Y + 16);
        }

        public static IEnumerable<Rectangle> GetPlayerBackgroundRectangles()
        {
            var rectangleList = new List<Rectangle>();

            for (int i = 0; i < ServerManager.PlayerCount; i++)
            {
                var middle = 1920 / 2;
                var x = middle;
                if (i == 0)
                {
                    x -= 288 + 96;
                }
                else if (i == 1)
                {
                    x -= 144 + 32;
                }
                else if (i == 2)
                {
                    x += 32;
                }
                else if (i == 3)
                {
                    x += 144 + 96;
                }

                rectangleList.Add(new Rectangle(x, 448, 144, 256));
            }

            return rectangleList;
        }

        public LobbyFrame CreateFrame()
        {
            var frame = new LobbyFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars.Values),
                PlayerBackgrounds = GetPlayerBackgroundRectangles().Select(RectangleJson.CreateFrom).ToList()
            };

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
    }
}
