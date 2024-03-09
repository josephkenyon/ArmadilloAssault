using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Battle.Environment.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Flows;
using ArmadilloAssault.GameState.Battle.Input;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.GameState.Menu;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Communication.Frame;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static Dictionary<PlayerIndex, Avatar> Avatars { get; set; } = [];
        public static BattleFrame BattleFrame { get; set; }
        private static Menu.Assets.Menu PauseMenu { get; set; }
        public static bool Paused { get; private set; }
        private static bool ServerPaused { get; set; }

        public static void Initialize(string data)
        {
            ModeManager.Clear();

            ServerPaused = false;
            Paused = ServerPaused;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);

            Scene = new Scene(sceneConfiguration);

            EffectManager.Initialize();
            EnvironmentalEffectsManager.Initialize(sceneConfiguration.EnvironmentalEffects);
            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager.Initialize(sceneConfiguration.Flow);
        }

        public static void Initialize(Dictionary<PlayerIndex, AvatarType> avatars, string data)
        {
            Paused = false;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);
            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

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

            CloudManager.Initialize(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager.Initialize(sceneConfiguration.Flow);

            ModeManager.Initialize(avatars.Keys);

            BattleFrame = CreateBattleFrame();
        }

        public static void UpdateServer()
        {
            if (ControlsManager.IsControlDownStart(0, Control.Start) && (!ModeManager.GameOver))
            {
                SetPaused(!Paused);
            }

            if (Paused)
            {
                PauseMenu.Update();

                if (ControlsManager.IsControlDownStart(0, Control.Confirm))
                {
                    var button = PauseMenu.Buttons.FirstOrDefault(button => button.Selected);

                    if (button != null)
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => MenuManager.InvokeAction(action, button.Data));
                    }
                }

                if (!ModeManager.GameOver)
                {
                    return;
                }
            }

            if (ModeManager.GameOver && !Paused)
            {
                SetPaused(true);
            }
            
            EffectManager.UpdateEffects();

            foreach (var avatarPair in Avatars)
            {
                var avatar = avatarPair.Value;

                if (!avatar.IsDead && !ModeManager.GameOver)
                {
                    InputManager.UpdateAvatar((int)avatarPair.Key, avatar);
                }

                PhysicsManager.Update(avatar, Scene.CollisionBoxes, Scene.Size);

                avatar.Update();
            }

            BulletManager.UpdateBullets([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();

            CrateManager.UpdateCrates([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            CameraManager.UpdateFocusPoint(Avatars.Values.First().Position);

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

            if (ServerManager.IsServing)
            {
                ServerManager.SendBattleFrame(BattleFrame, hudFrames);
            }

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
            if (ControlsManager.IsControlDownStart(0, Control.Start))
            {
                SetPaused(!Paused);
            }

            if (Paused)
            {
                PauseMenu.Update();

                if (ControlsManager.IsControlDownStart(0, Control.Confirm))
                {
                    var button = PauseMenu.Buttons.FirstOrDefault(button => button.Selected);

                    if (button != null)
                    {
                        SoundManager.PlayMenuSound(MenuSound.confirm);
                        button.Actions.ForEach(action => MenuManager.InvokeAction(action, button.Data));
                    }
                }

                return;
            }

            if (Paused)
            {
                PauseMenu.Update();
            }

            if (!ServerPaused)
            {
                EnvironmentalEffectsManager.UpdateEffects();
                CloudManager.UpdateClouds();
                FlowManager.UpdateFlows();
            }
        }

        public static void Draw()
        {
            GraphicsManager.Clear(Scene.BackgroundColor);

            DrawingManager.DrawCollection(FlowManager.Flows);

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f, CameraManager.GetBackgroundSourceRectangle());

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

            if (Paused)
            {
                DrawingManager.DrawMenuButtons(PauseMenu.Buttons.Where(button => button.Visible));
                return;
            }

            DrawingManager.DrawTexture(TextureName.crosshair, CameraManager.CursorPosition - new Vector2(16, 16));
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

        public static void SetPaused(bool paused, bool enforcedByServer = false)
        {
            Paused = paused;
            if (Paused)
            {
                var menuName = ModeManager.GameOver ? "Game_Over" : "Pause";
                PauseMenu = new Menu.Assets.Menu(ConfigurationManager.GetMenuConfiguration(menuName), true);
            }

            if (ServerManager.IsServing)
            {
                ServerManager.BroadcastPause(paused);
            }
            else if (ClientManager.IsActive)
            {
                if (!enforcedByServer)
                {
                    _ = ClientManager.BroadcastPausedChange(paused);
                }
                else if (!ModeManager.GameOver)
                {
                    ServerPaused = paused;
                }
            }
        }

        public static void PlayerDisconnected(int index)
        {
            Avatars.Remove((PlayerIndex)index);
        }
    }
}
