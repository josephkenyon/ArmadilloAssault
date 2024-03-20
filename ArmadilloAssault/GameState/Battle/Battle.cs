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
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Environment.Precipitation;

namespace ArmadilloAssault.GameState.Battle
{
    public class Battle : IModeManagerListener, IAvatarListener, IBulletListener, IWeaponListener
    {
        public Dictionary<int, Avatar> Avatars { get; set; } = [];

        public Scene Scene { get; set; }
        public BulletManager BulletManager { get; set; }
        public CloudManager CloudManager { get; set; }
        public PrecipitationManager PrecipitationManager { get; set; }
        public CrateManager CrateManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public EnvironmentalEffectsManager EnvironmentalEffectsManager { get; set; }
        public FlowManager FlowManager { get; set; }
        public ModeManager ModeManager { get; set; }
        public BattleFrame Frame { get; set; }

        public bool Paused { get; set; }
        public bool GameOver { get; set; }

        public readonly int PlayerIndex;

        public ModeType? Mode => ModeManager?.Mode;

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
            PrecipitationManager = new(Scene.Size, sceneConfiguration.PrecipitationType);
        }

        public Battle(Dictionary<int, AvatarType> avatars, Dictionary<int, int> playerTeamRelations, Dictionary<int, AvatarProp> avatarProps, ModeType mode, string sceneName) {
            PlayerIndex = 0;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(sceneName);
            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

            foreach (var index in avatars.Keys)
            {
                var crowned = false;
                var avatarProp = avatarProps[index];
                if (avatarProp != null)
                {
                    crowned = avatarProp.Crowned;
                }

                var avatar = new Avatar(index, ConfigurationManager.GetAvatarConfiguration(avatars[index]), this, crowned);
                avatar.SetStartingPosition(Scene.StartingPositions[index]);

                Avatars.Add(index, avatar);
            }

            if (ModeType.Tutorial == mode)
            {
                Avatars.Values.ElementAt(0).SetStartingPosition(new Vector2(900, 580));
            }

            BulletManager = new(Scene.CollisionBoxes.Where(box => box.Height > CollisionHelper.PassableYThreshold).ToList(), Scene.Size, this);
            CrateManager = new(Scene.CollisionBoxes, Scene.Size);

            EffectManager = new();

            EnvironmentalEffectsManager = new(sceneConfiguration.EnvironmentalEffects);

            CloudManager = new(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager = new(sceneConfiguration.Flow);
            PrecipitationManager = new(Scene.Size, sceneConfiguration.PrecipitationType);

            ModeManager = new(this, avatars.Keys.Select(key => new KeyValuePair<int, int>(key, playerTeamRelations[key])), mode);

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
                    InputManager.UpdateAvatar(avatarPair.Key, avatar);
                }

                PhysicsManager.Update(avatar, Scene);

                avatar.Update();
            }

            BulletManager?.UpdateBullets([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();
            PrecipitationManager.UpdatePrecipitation();

            CrateManager?.UpdateCrates([.. Avatars.Values.Where(avatar => !avatar.IsDead)]);

            if (Avatars.Count > 0)
            {
                CameraManager.UpdateFocusPoint(Avatars[PlayerIndex].Position);
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

            if (ModeManager != null && ModeManager.Mode == ModeType.King_of_the_Hill)
            {
                ModeManager.UpdateKingOfTheHill((Rectangle)Scene.CapturePoint, Avatars.Values);

                if (!GameOver && ModeManager.GameOver)
                {
                    BattleManager.SetGameOver();
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

            if (Scene.BackBackgroundTexture != TextureName.nothing)
            {
                DrawingManager.DrawTexture(Scene.BackBackgroundTexture, new Rectangle(0, 0, 1920, 1080), 0.75f, CameraManager.GetBackgroundSourceRectangle(true));
            }

            DrawingManager.DrawTexture(
                Scene.BackgroundTexture, new Rectangle(0, 0, 1920, 1080),
                Scene.BackBackgroundTexture != TextureName.nothing ? 1f : 0.75f,
                CameraManager.GetBackgroundSourceRectangle()
            );

            DrawingManager.DrawCollection(EnvironmentalEffectsManager.Effects);

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => !cloud.Foreground));

            DrawingManager.DrawCollection(PrecipitationManager.BackgroundParticles);

            foreach (var list in Scene.TileLists.Where(list => list.Z < 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (Frame != null && Frame.HudFrame != null && Frame.HudFrame.CapturePointColor != null && Scene.CapturePoint != null)
            {
                var rectangle = new Rectangle(
                    Scene.CapturePoint.Value.X - CameraManager.Offset.X,
                    Scene.CapturePoint.Value.Y - CameraManager.Offset.Y,
                    Scene.CapturePoint.Value.Width,
                    Scene.CapturePoint.Value.Height
                );

                DrawingManager.DrawRectangles(new List<Rectangle> { rectangle }, Frame.HudFrame.CapturePointColor.ToColor());
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
            DrawingManager.DrawCollection(PrecipitationManager.ForegroundParticles);

            if (Frame != null)
            {
                var offset = new Vector2(64, -48);
                DrawingManager.DrawStrings(Frame.AvatarFrame.RespawnTimers, Frame.AvatarFrame.Positions.Select(position => position + offset - CameraManager.Offset.ToVector2()));

                if (Frame.HudFrame != null)
                {
                    DrawingManager.DrawHud(Frame.HudFrame, PlayerIndex);

                    if (Frame.HudFrame.CapturePointColor != null && Scene.CapturePoint != null && Frame.HudFrame.CapturePointSeconds != null)
                    {
                        var rectangle = new Rectangle(
                            Scene.CapturePoint.Value.X - CameraManager.Offset.X,
                            Scene.CapturePoint.Value.Y - CameraManager.Offset.Y,
                            Scene.CapturePoint.Value.Width,
                            Scene.CapturePoint.Value.Height
                        );

                        DrawingManager.DrawString(Frame.HudFrame.CapturePointSeconds.ToString(), rectangle.Center.ToVector2(), DrawingHelper.MediumFont);
                    }
                }
            }

            if (!BattleManager.Paused && !GameOver)
            {
                DrawingManager.DrawTexture(TextureName.crosshair, CameraManager.CursorPosition - new Vector2(16, 16));
            }

            if (GameOver && Frame != null && Frame.GameOverMessage != null)
            {
                DrawingManager.DrawString(Frame.GameOverMessage, new Vector2(960, 360), DrawingHelper.MediumFont);
            }

            if (ModeType.Tutorial == Mode && !Paused)
            {
                DrawingManager.DrawBattleTooltips([
                    ConfigurationManager.GetToolTip("movement"),
                    ConfigurationManager.GetToolTip("firing"),
                    ConfigurationManager.GetToolTip("weapon_crates"),
                    ConfigurationManager.GetToolTip("power_ups"),
                ]);
            }
        }

        private BattleFrame CreateFrame()
        {
            var battleFrame = new BattleFrame
            {
                GameOverMessage = ModeManager.VictoryMessage,
                AvatarFrame = AvatarFrame.CreateFrom(Avatars, ModeManager.PlayerTeamRelations, showCrowns: ModeType.Regicide == Mode),
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

            if (ModeType.Tutorial != Mode)
            {
                hudFrame.ModeType = ModeManager.Mode;
                hudFrame.TeamIndices = [.. ModeManager.PlayerTeamRelations.Values.Order()];
                hudFrame.ModeValues = ModeManager.GetModeValues();
            }

            foreach (var avatarKey in Avatars.Keys)
            {
                var avatar = Avatars[avatarKey];

                var weapon = avatar.SelectedWeapon;

                hudFrame.PlayerIndices.Add(avatarKey);
                hudFrame.Deads.Add(avatar.IsDead);
                hudFrame.Visibles.Add(PowerUpType.Invisibility != avatar.CurrentPowerUp);
                hudFrame.AvatarXs.Add((int)avatar.Position.X);
                hudFrame.AvatarYs.Add((int)avatar.Position.Y);
                hudFrame.Healths.Add(avatar.Health);
                hudFrame.Ammos.Add(weapon.AmmoInClip + weapon.Ammo);
            }

            if (ModeType.King_of_the_Hill == Mode)
            {
                hudFrame.CapturePointColor = ColorJson.CreateFrom(ModeManager.GetCapturePointColor());
                hudFrame.CapturePointSeconds = ModeManager.CaputurePointSeconds;
            }

            return hudFrame;
        }

        public void SetRespawnTimer(int avatarIndex, int frames)
        {
            Avatars[avatarIndex].RespawnTimerFrames = frames;
        }

        public void AvatarHit(int hitIndex, int firedIndex, int damage)
        {
            ModeManager.AvatarHit(hitIndex, firedIndex, damage);
        }

        public void AvatarKilled(int deadIndex, int? killIndex)
        {
            if (GameOver) return;

            CrateManager.CreateNewCrate(CrateType.Weapon, Avatars[deadIndex].SelectedWeapon.Type, Avatars[deadIndex].Position, true);

            ModeManager.AvatarKilled(deadIndex, killIndex);

            if (ModeManager.GameOver)
            {
                BattleManager.SetGameOver();
            }
            else if (ModeManager.GetPlayerShouldRespawn(deadIndex))
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

            DrawingManager.DrawRectangles(boxList);
        }

        public int GetTeamIndex(int playerIndex)
        {
            return ModeManager.GetTeamIndex(playerIndex);
        }

        public Dictionary<int, Avatar> GetAvatars() => Avatars;
    }
}
