using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Bullets;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Clouds;
using ArmadilloAssault.GameState.Battle.Environment.Effects;
using ArmadilloAssault.GameState.Battle.Environment.Flows;
using ArmadilloAssault.GameState.Battle.Mode;
using ArmadilloAssault.Graphics.Drawing.Avatars;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using ArmadilloAssault.GameState.Battle.Input;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Server;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.PowerUps;
using Microsoft.Xna.Framework.Input;

namespace ArmadilloAssault.GameState.Battle
{
    public class Battle : IAvatarListener, IBulletListener, IWeaponListener
    {
        public Dictionary<PlayerIndex, Avatar> Avatars { get; set; } = [];

        public Scene Scene { get; set; }
        public BulletManager BulletManager { get; set; }
        public CloudManager CloudManager { get; set; }
        public CrateManager CrateManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public EnvironmentalEffectsManager EnvironmentalEffectsManager { get; set; }
        public FlowManager FlowManager { get; set; }
        public ModeManager ModeManager { get; set; }
        public BattleFrame Frame { get; set; }

        public bool Paused { get; set; }
        public bool GameOver { get; set; }

        public readonly int PlayerIndex;

        public Battle(string data, int playerIndex)
        {
            PlayerIndex = playerIndex;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);

            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

            EffectManager = new();
            EnvironmentalEffectsManager = new(sceneConfiguration.EnvironmentalEffects);
            CloudManager = new(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager = new(sceneConfiguration.Flow);
        }

        public Battle(Dictionary<PlayerIndex, AvatarType> avatars, string data) {
            PlayerIndex = 0;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(data);
            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

            foreach (var index in avatars.Keys)
            {
                Avatars.Add(index, new Avatar((int)index, ConfigurationManager.GetAvatarConfiguration(avatars[index]), this));
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

            BulletManager = new(Scene.CollisionBoxes.Where(box => box.Height > CollisionHelper.PassableYThreshold).ToList(), Scene.Size, this);
            CrateManager = new(Scene.CollisionBoxes);

            EffectManager = new();

            EnvironmentalEffectsManager = new(sceneConfiguration.EnvironmentalEffects);

            CloudManager = new(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager = new(sceneConfiguration.Flow);

            ModeManager = new(avatars.Keys);

            Frame = CreateFrame();
        }
        public void Update()
        {
            CameraManager.UpdateFocusOffset(Mouse.GetState().Position.ToVector2());

            EffectManager?.UpdateEffects();

            foreach (var avatarPair in Avatars)
            {
                var avatar = avatarPair.Value;

                if (!avatar.IsDead && !GameOver)
                {
                    InputManager.UpdateAvatar((int)avatarPair.Key, avatar);
                }

                PhysicsManager.Update(avatar, Scene);

                avatar.Update();
            }

            BulletManager?.UpdateBullets([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();

            CrateManager?.UpdateCrates([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            if (Avatars.Count > 0)
            {
                CameraManager.UpdateFocusPoint(Avatars[(PlayerIndex)PlayerIndex].Position);
            }

            if (Avatars.Count > 0)
            {
                Frame = CreateFrame();
                
                SoundManager.PushSounds(Frame);

                if (ServerManager.IsServing)
                {
                    ServerManager.SendBattleFrame(Frame);
                }
            }
        }

        public void Draw()
        {
            if (Frame != null)
            {
                var index = Frame.AvatarFrame.PlayerIndices.IndexOf(PlayerIndex);
                CameraManager.UpdateFocusPoint(Frame.AvatarFrame.Positions[index]);
            }

            GraphicsManager.Clear(Scene.BackgroundColor);

            DrawingManager.DrawCollection(FlowManager.Flows);

            DrawingManager.DrawTexture(Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f, CameraManager.GetBackgroundSourceRectangle());

            DrawingManager.DrawCollection(EnvironmentalEffectsManager.Effects);

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => !cloud.Foreground));

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (Frame != null)
            {
                SoundManager.PlaySounds(Frame.SoundFrame);

                DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(Frame.AvatarFrame, PlayerIndex));
                DrawingManager.DrawCollection(CrateManager.GetDrawableCrates(Frame.CrateFrame));
            }

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (Frame != null)
            {
                DrawingManager.DrawCollection(BulletManager.GetDrawableBullets(Frame.BulletFrame));
                DrawingManager.DrawCollection(EffectManager.GetDrawableEffects(Frame.EffectFrame));
            }

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => cloud.Foreground));

            if (Frame != null)
            {
                var offset = new Vector2(64, -48);
                DrawingManager.DrawStrings(Frame.AvatarFrame.RespawnTimers, Frame.AvatarFrame.Positions.Select(position => position + offset - CameraManager.Offset.ToVector2()));

                if (Frame.HudFrame != null)
                {
                    DrawingManager.DrawHud(Frame.HudFrame, PlayerIndex);
                }
            }

            if (!BattleManager.Paused && !GameOver)
            {
                DrawingManager.DrawTexture(TextureName.crosshair, CameraManager.CursorPosition - new Vector2(16, 16));
            }
        }

        private BattleFrame CreateFrame()
        {
            var battleFrame = new BattleFrame
            {
                AvatarFrame = AvatarFrame.CreateFrom(Avatars),
                BulletFrame = BulletManager.GetBulletFrame(),
                CrateFrame = CrateManager.GetCrateFrame(),
                EffectFrame = EffectManager.GetEffectFrame(),
                HudFrame = CreateHudFrame()
            };

            return battleFrame;
        }

        private HudFrame CreateHudFrame()
        {
            var hudFrame = new HudFrame();

            foreach (var avatarKey in Avatars.Keys)
            {
                var avatar = Avatars[avatarKey];

                var weapon = avatar.SelectedWeapon;

                hudFrame.PlayerIndices.Add((int)avatarKey);
                hudFrame.Deads.Add(avatar.IsDead);
                hudFrame.Visibles.Add(PowerUpType.Invisibility != avatar.CurrentPowerUp);
                hudFrame.AvatarXs.Add((int)avatar.Position.X);
                hudFrame.AvatarYs.Add((int)avatar.Position.Y);
                hudFrame.Healths.Add(avatar.Health);
                hudFrame.Ammos.Add(weapon.AmmoInClip + weapon.Ammo);
            }

            return hudFrame;
        }

        public void SetRespawnTimer(int avatarIndex, int frames)
        {
            Avatars[(PlayerIndex)avatarIndex].RespawnTimerFrames = frames;
        }

        public void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            ModeManager.AvatarHit(hitIndex, firedIndex, damage);
        }

        public void AvatarKilled(int deadIndex, int? killIndex)
        {
            ModeManager.AvatarKilled(deadIndex, killIndex);

            if (ModeManager.GameOver)
            {
                BattleManager.SetGameOver();
            }
            else
            {
                SetRespawnTimer(deadIndex, ModeManager.RespawnFrames);
            }
        }

        public void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null)
        {
            EffectManager?.CreateEffect(position, effectType, direction, weaponAngle);
        }

        public void CreateBullet(WeaponJson weaponConfiguration, Vector2 position, float angleTrajectory, float damageModifier, int playerIndex)
        {
            BulletManager?.CreateBullet(weaponConfiguration, position, angleTrajectory, damageModifier, playerIndex);
        }

        private void DrawCollisionBoxes()
        {
            var boxList = new List<Rectangle>();
            var boxesList = Avatars.Values.Select(avatar => avatar.GetHurtBoxes());
            foreach (var boxes in boxesList)
            {
                boxList.AddRange(boxes);
            }

            boxList.AddRange(Avatars.Values.Select(avatar => avatar.GetShellBox()));

            DrawingManager.DrawCollisionBoxes(boxList);
        }
    }
}
