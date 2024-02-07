using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.Controls;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Drawing;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.GameState.Battle.Input;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.GameState.Battle.Players;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;
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

            Avatars.Values.First().SetPosition(new Vector2(1000, 0));

            BulletManager.Initialize(Scene.CollisionBoxes);
            EffectManager.Initialize();
        }

        public static void Update(Action exit)
        {
            foreach (var avatar in Avatars)
            {
                InputManager.UpdateAvatar((int)avatar.Key, avatar.Value);
                PhysicsManager.UpdateAvatar(avatar.Value, Scene.CollisionBoxes);

                avatar.Value.Update();
            }

            BulletManager.UpdateBullets();
            EffectManager.UpdateEffects();

            if (ControlsManager.IsControlDown(0, Control.Start))
            {
                exit.Invoke();
            }
        }

        public static void Draw()
        {
            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080));

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            BattleDrawingHelper.DrawAvatars(Avatars.Values);

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            BattleDrawingHelper.DrawBullets();
            BattleDrawingHelper.DrawEffects();
        }
    }
}
