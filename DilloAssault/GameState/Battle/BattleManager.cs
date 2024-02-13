﻿using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.Configuration.Avatars;
using DilloAssault.Controls;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Bullets;
using DilloAssault.GameState.Battle.Crates;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.GameState.Battle.Environment.Clouds;
using DilloAssault.GameState.Battle.Input;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Generics;
using DilloAssault.Graphics.Drawing;
using DilloAssault.Web.Client;
using DilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; }

        public static void Initialize(int playerCount)
        {
            Scene = new Scene(ConfigurationManager.GetSceneConfiguration());
            Avatars = [];

            Avatars.Add(PlayerIndex.One, new Avatar(ConfigurationManager.GetAvatarConfiguration(AvatarType.Arthur)));

            if (playerCount == 2)
            {
                Avatars.Add(PlayerIndex.Two, new Avatar(ConfigurationManager.GetAvatarConfiguration(AvatarType.Axel)));
            }

            Avatars.Values.First().SetPosition(new Vector2(1000, 0));
            Avatars.Values.Last().SetPosition(new Vector2(500, 0));

            BulletManager.Initialize(Scene.CollisionBoxes);
            CrateManager.Initialize(Scene.CollisionBoxes);
            EffectManager.Initialize();
            CloudManager.Initialize();
        }

        public static void Update()
        {
            EffectManager.UpdateEffects();

            var index = 0;
            foreach (var avatar in Avatars)
            {
                if (ServerManager.IsServing)
                {
                    InputManager.UpdateAvatar((int)avatar.Key, avatar.Value);
                    PhysicsManager.Update(avatar.Value, Scene.CollisionBoxes);
                    avatar.Value.Update();
                }
                else if (ClientManager.AvatarUpdates.Count > index)
                {
                    var avatarUpdate = ClientManager.AvatarUpdates[index];

                    avatar.Value.Update(avatarUpdate);
                }

                index++;
            }

            if (ServerManager.IsServing)
            {
                BulletManager.UpdateBullets([.. Avatars.Values]);
            }

            CloudManager.UpdateClouds();

            if (ServerManager.IsServing)
            {
                CrateManager.UpdateCrates(Avatars.Values);
            }

            if (ControlsManager.IsControlDown(0, Control.Start))
            {
                GameStateManager.State = State.Menu;
            }
            else if (ServerManager.IsServing)
            {
                ServerManager.SendBattleUpdates();
            }
        }

        public static void Draw()
        {
            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f);

            DrawingManager.DrawCollection(CloudManager.Clouds);

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            DrawingManager.DrawCollection(GetAvatars());

            DrawingManager.DrawCollection(CrateManager.Crates);

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            DrawingManager.DrawCollection(BulletManager.Bullets);

            DrawingManager.DrawCollection(EffectManager.Effects);
        }

        private static List<IDrawableObject> GetAvatars()
        {
            var avatarCollection = new List<IDrawableObject>();

            var notSpinningAvatars = Avatars.Values.Where(avatar => !avatar.IsSpinning).ToList();
            var spinningAvatars = Avatars.Values.Where(avatar => avatar.IsSpinning).ToList();

            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetArm(avatar, Direction.Left)));

            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetLeg(avatar, Direction.Left)));

            foreach (var avatar in Avatars.Values)
            {
                avatarCollection.Add(AvatarDrawingHelper.GetBody(avatar));
            }

            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetLeg(avatar, Direction.Right)));

            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetHead(avatar)));
            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetGun(avatar)));
            notSpinningAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetArm(avatar, Direction.Right)));

            return avatarCollection;
        }
    }
}
