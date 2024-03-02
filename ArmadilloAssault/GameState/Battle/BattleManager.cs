using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Battle.Environment.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Flows;
using ArmadilloAssault.GameState.Battle.Input;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; } = [];
        public static BattleFrame BattleFrame { get; set; }

        public static void Initialize(string data)
        {
            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);

            Scene = new Scene(sceneConfiguration);

            EffectManager.Initialize();
            EnvironmentalEffectsManager.Initialize(sceneConfiguration.EnvironmentalEffects);
            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly);
            FlowManager.Initialize(sceneConfiguration.Flow);
        }

        public static void Initialize(Dictionary<PlayerIndex, AvatarType> avatars, string data)
        {
            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);
            Scene = new Scene(sceneConfiguration);
            Avatars.Clear();

            foreach (var index in avatars.Keys)
            {
                Avatars.Add(index, new Avatar((int)index, ConfigurationManager.GetAvatarConfiguration(avatars[index])));
            }

            Avatars.Values.First().SetStartingPosition(Scene.StartingPositions.First.ToVector2());

            if (Avatars.Values.Count >= 2)
            {
                Avatars.Values.ElementAt(1).SetStartingPosition(Scene.StartingPositions.Second.ToVector2());
            }

            if (Avatars.Values.Count >= 3)
            {
                Avatars.Values.ElementAt(2).SetStartingPosition(Scene.StartingPositions.Third.ToVector2());
            }

            if (Avatars.Values.Count >= 4)
            {
                Avatars.Values.ElementAt(3).SetStartingPosition(Scene.StartingPositions.Fourth.ToVector2());
            }

            BulletManager.Initialize(Scene.CollisionBoxes);
            CrateManager.Initialize(Scene.CollisionBoxes);

            EffectManager.Initialize();

            EnvironmentalEffectsManager.Initialize(sceneConfiguration.EnvironmentalEffects);

            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly);
            FlowManager.Initialize(sceneConfiguration.Flow);

            ModeManager.Initialize(avatars.Keys);

            BattleFrame = CreateBattleFrame();
        }

        public static void UpdateServer()
        {
            EffectManager.UpdateEffects();

            foreach (var avatarPair in Avatars)
            {
                var avatar = avatarPair.Value;

                if (!avatar.IsDead)
                {
                    InputManager.UpdateAvatar((int)avatarPair.Key, avatar);
                }

                PhysicsManager.Update(avatar, Scene.CollisionBoxes);

                avatar.Update();
            }

            BulletManager.UpdateBullets([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();

            CrateManager.UpdateCrates([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            if (ControlsManager.IsControlDown(0, Control.Start))
            {
                ServerManager.EndGame();
                GameStateManager.State = State.Menu;
                return;
            }

            BattleFrame = CreateBattleFrame();
            var hudFrames = Avatars.Values.Select(avatar =>
            {
                var weapon = avatar.SelectedWeapon;
                return new HudFrame
                {
                    AvatarX = (int)avatar.Position.X,
                    AvatarY = (int)avatar.Position.Y,
                    Health = avatar.Health,
                    Ammo = weapon.AmmoInClip + weapon.Ammo
                };
            });

            SoundManager.PushSounds(BattleFrame);

            ServerManager.SendBattleFrame(BattleFrame, hudFrames);

            var myIndex = BattleFrame.AvatarFrame.PlayerIndices.FindIndex(index => index == 0);
            BattleFrame.HudFrame = hudFrames.ElementAt(myIndex);

            var index = 0;
            BattleFrame.AvatarFrame.Colors.ForEach(color =>
            {
                color.A = MathUtils.GetAlpha(color, index++, myIndex);
            });
        }

        public static void UpdateClient()
        {
            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();
        }

        public static void Draw()
        {
            GraphicsManager.Clear(Scene.BackgroundColor);

            DrawingManager.DrawCollection(FlowManager.Flows);

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f);

            DrawingManager.DrawCollection(EnvironmentalEffectsManager.Effects);

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => !cloud.Foreground));

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (BattleFrame != null)
            {
                SoundManager.PlaySounds(BattleFrame.SoundFrame);

                DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(BattleFrame.AvatarFrame));
                DrawingManager.DrawCollection(CrateManager.GetDrawableCrates(BattleFrame.CrateFrame));
            }

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (BattleFrame != null)
            {
                DrawingManager.DrawCollection(BulletManager.GetDrawableBullets(BattleFrame.BulletFrame));
                DrawingManager.DrawCollection(EffectManager.GetDrawableEffects(BattleFrame.EffectFrame));
            }

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => cloud.Foreground));

            /**
            var boxList = new List<Rectangle>();
            var boxesList = Avatars.Values.Select(avatar => avatar.GetHurtBoxes());
            foreach (var boxes in boxesList)
            {
                boxList.AddRange(boxes);
            }

            boxList.AddRange(Avatars.Values.Select(avatar => avatar.GetShellBox()));

            DrawingManager.DrawCollisionBoxes(boxList);
            **/


            if (BattleFrame != null)
            {
                var vector = new Vector2(64, 64 - 16);
                DrawingManager.DrawStrings(BattleFrame.AvatarFrame.RespawnTimers, BattleFrame.AvatarFrame.Positions.Select(position => position + vector));

                if (BattleFrame.HudFrame != null)
                {
                    DrawingManager.DrawHud(BattleFrame.HudFrame);
                }
            }
        }

        private static BattleFrame CreateBattleFrame()
        {
            var battleFrame = new BattleFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars),
                BulletFrame = BulletManager.GetBulletFrame(),
                CrateFrame = CrateManager.GetCrateFrame(),
                EffectFrame = EffectManager.GetEffectFrame(),
            };

            return battleFrame;
        }

        public static void SetRespawnTimer(int avatarIndex, int frames)
        {
            Avatars[(PlayerIndex)avatarIndex].RespawnTimerFrames = frames;
        }
    }
}
