using DilloAssault.Assets;
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
using DilloAssault.Web.Communication.Updates;
using DilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; }
        public static int AvatarIndex { get; private set; }

        public static void Initialize(int playerCount, int avatarIndex = 0)
        {
            Scene = new Scene(ConfigurationManager.GetSceneConfiguration());
            Avatars = [];

            AvatarIndex = avatarIndex;

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

        public static void UpdateServer()
        {
            EffectManager.UpdateEffects();

            var index = 0;

            foreach (var avatarPair in Avatars)
            {
                var avatar = avatarPair.Value;

                if (!avatar.IsDead)
                {
                    InputManager.UpdateAvatar(index, avatar);
                }

                PhysicsManager.Update(avatar, Scene.CollisionBoxes);


                if (!avatar.IsDead)
                {
                    avatar.Update();
                }

                index++;
            }

            BulletManager.UpdateBullets([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            CloudManager.UpdateClouds();

            CrateManager.UpdateCrates([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            if (ControlsManager.IsControlDown(0, Control.Start))
            {
                ServerManager.EndGame();
                GameStateManager.State = State.Menu;
            }
            else
            {
                ServerManager.SendBattleUpdates();
            }
        }

        public static void UpdateClient()
        {
            CloudManager.UpdateClouds();

            FrameUpdate frame = null;
            if (ClientManager.Frames.Count > 4)
            {
                frame = ClientManager.Frames.Last();
                ClientManager.Frames.Clear();
            }

            if (ClientManager.Frames.Count > 0)
            {
                frame = ClientManager.Frames.Dequeue();
            }

            if (frame == null)
            {
                return;
            }

            for (int i = 0; i < frame.AvatarUpdates.Count; i++)
            {
                var avatar = Avatars.Values.ElementAt(i);
                if (i == AvatarIndex && !avatar.IsDead)
                {
                    InputManager.UpdateAvatar(i, avatar);
                }

                avatar.Update(frame.AvatarUpdates[i]);
            }

            EffectManager.UpdateEffects(frame.EffectsUpdate);
            CrateManager.UpdateCrates(frame.CratesUpdate);
            BulletManager.UpdateBullets(frame.BulletsUpdate);
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

            var notSpinningOrDeadAvatars = Avatars.Values.Where(avatar => !avatar.IsSpinning && !avatar.IsDead).ToList();

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetArm(avatar, Direction.Left)));

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetLeg(avatar, Direction.Left)));

            foreach (var avatar in Avatars.Values)
            {
                avatarCollection.Add(AvatarDrawingHelper.GetBody(avatar));
            }

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetLeg(avatar, Direction.Right)));

            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetHead(avatar)));
            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetGun(avatar)));
            notSpinningOrDeadAvatars.ForEach(avatar => avatarCollection.Add(AvatarDrawingHelper.GetArm(avatar, Direction.Right)));

            return avatarCollection;
        }
    }
}
