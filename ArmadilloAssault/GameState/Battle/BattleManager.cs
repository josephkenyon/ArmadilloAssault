using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Battle.Environment.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Flows;
using ArmadilloAssault.GameState.Battle.Input;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
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
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; }
        public static BattleFrame BattleFrame { get; set; }
        public static int AvatarIndex { get; private set; }

        public static void Initialize(int playerCount, string data, int avatarIndex = 0)
        {
            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);
            Scene = new Scene(sceneConfiguration);
            Avatars = [];

            AvatarIndex = avatarIndex;

            Avatars.Add(PlayerIndex.One, new Avatar(ConfigurationManager.GetAvatarConfiguration(AvatarType.Titan)));
            Avatars.Values.First().SetPosition(new Vector2(150, 0));

            if (playerCount >= 2)
            {
                Avatars.Add(PlayerIndex.Two, new Avatar(ConfigurationManager.GetAvatarConfiguration(AvatarType.Axel)));
                Avatars.Values.Last().SetPosition(new Vector2(1650, 0));
                Avatars.Values.Last().SetDirection(Direction.Left);
            }

            if (playerCount >= 3)
            {
                Avatars.Add(PlayerIndex.Three, new Avatar(ConfigurationManager.GetAvatarConfiguration(AvatarType.Titan)));
                Avatars.Values.Last().SetPosition(new Vector2(420, 750));
                Avatars.Values.Last().SetDirection(Direction.Right);
            }

            BulletManager.Initialize(Scene.CollisionBoxes);
            CrateManager.Initialize(Scene.CollisionBoxes);
            EffectManager.Initialize();
            EnvironmentalEffectsManager.Initialize(sceneConfiguration.EnvironmentalEffects);
            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly);
            FlowManager.Initialize(sceneConfiguration.Flow);
            AvatarDrawingHelper.Initialize();

            BattleFrame = CreateBattleFrame();
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

                avatar.Update();

                index++;
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

            SoundManager.AddSounds(BattleFrame);

            ServerManager.SendBattleFrame(BattleFrame, hudFrames);

            BattleFrame.HudFrame = hudFrames.First();
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

            SoundManager.PlaySounds(BattleFrame.SoundFrame);

            DrawingManager.DrawCollection(FlowManager.Flows);

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f);

            DrawingManager.DrawCollection(EnvironmentalEffectsManager.Effects);

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => !cloud.Foreground));

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(BattleFrame.AvatarFrame));

            DrawingManager.DrawCollection(CrateManager.GetDrawableCrates(BattleFrame.CrateFrame));

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            DrawingManager.DrawCollection(BulletManager.GetDrawableBullets(BattleFrame.BulletFrame));

            DrawingManager.DrawCollection(EffectManager.GetDrawableEffects(BattleFrame.EffectFrame));

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

            if (BattleFrame.HudFrame != null)
            {
                DrawingManager.DrawHud(BattleFrame.HudFrame);
            }
        }

        private static BattleFrame CreateBattleFrame()
        {
            var battleFrame = new BattleFrame();

            foreach (var avatar in Avatars.Values)
            {
                battleFrame.AvatarFrame.Animations.Add(avatar.Animation);
                battleFrame.AvatarFrame.ArmAngles.Add((float)avatar.ArmAngle);
                battleFrame.AvatarFrame.AnimationFrames.Add(avatar.AnimationFrame);
                battleFrame.AvatarFrame.BreathingYOffsets.Add(avatar.GetBreathingYOffset());
                battleFrame.AvatarFrame.Deads.Add(avatar.IsDead);
                battleFrame.AvatarFrame.Directions.Add(avatar.Direction);
                battleFrame.AvatarFrame.Positions.Add(avatar.Position);
                battleFrame.AvatarFrame.Recoils.Add(avatar.GetRecoil);
                battleFrame.AvatarFrame.Rotations.Add(avatar.Rotation);
                battleFrame.AvatarFrame.Spinnings.Add(avatar.IsSpinning);
                battleFrame.AvatarFrame.TextureNames.Add(avatar.TextureName);
                battleFrame.AvatarFrame.Types.Add(avatar.Type);
                battleFrame.AvatarFrame.WeaponTextures.Add(avatar.CurrentWeaponConfiguration.TextureName);
            }

            battleFrame.BulletFrame = BulletManager.GetBulletFrame();
            battleFrame.CrateFrame = CrateManager.GetCrateFrame();
            battleFrame.EffectFrame = EffectManager.GetEffectFrame();

            return battleFrame;
        }
    }
}
