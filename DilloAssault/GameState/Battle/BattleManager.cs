using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Input;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.GameState.Battle.Players;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static List<Player> Players { get; set; }
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; }

        public static void Initialize()
        {
            Players = [
                new Player { Name = "Player1", ConnectionId = null, PlayerControllerIndex = -1, PlayerIndex = 1 }
            ];

            Avatars = [];
            Avatars.Add(PlayerIndex.One, new Avatar(ConfigurationManager.GetAvatarConfiguration()));

            Avatars.Values.First().Position = new Vector2(606, 885);
        }

        public static void Update()
        {
            foreach (var avatar in Avatars)
            {
                InputManager.UpdateAvatar((int)avatar.Key, avatar.Value);
                PhysicsManager.UpdateAvatar(avatar.Value, Scene.CollisionBoxes);

                avatar.Value.Update();
            }
        }

        public static void Draw()
        {
            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            foreach (var avatar in Avatars.Values)
            {
                DrawingManager.DrawAvatarBackground(avatar);

                DrawingManager.DrawAvatarForeground(avatar);
            }

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }
        }
    }
}
