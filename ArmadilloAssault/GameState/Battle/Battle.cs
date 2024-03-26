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
using Microsoft.Xna.Framework.Input;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.GameState.Battle.Environment.Precipitation;
using System;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Controls;
using ArmadilloAssault.Web.Communication.Update;

namespace ArmadilloAssault.GameState.Battle
{
    public class Battle : IModeManagerListener, ICrateManagerListener, IEffectManagerListener, IAvatarListener, IBulletManagerListener, IWeaponListener, IItemListener
    {
        public Dictionary<int, Avatar> Avatars { get; set; }

        public Scene Scene { get; set; }
        public BulletManager BulletManager { get; set; }
        public CloudManager CloudManager { get; set; }
        public PrecipitationManager PrecipitationManager { get; set; }
        public CrateManager CrateManager { get; set; }
        public ItemManager ItemManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public EnvironmentalEffectsManager EnvironmentalEffectsManager { get; set; }
        public FlowManager FlowManager { get; set; }
        public ModeManager ModeManager { get; set; }

        public BattleStaticData BattleStaticData { get; set; }
        public BattleFrame Frame { get; set; }
        public StatFrame StatFrame { get; set; }

        public Queue<BattleUpdate> UpdateQueue { get; set; } = [];
        public BattleUpdate BattleUpdate { get; set; }

        public bool Paused { get; set; }
        public bool GameOver { get; set; }

        private int SpawnLocationIndex { get; set; }

        public ModeType Mode => BattleStaticData != null ? BattleStaticData.ModeType : ModeManager.Mode;

        public readonly bool Hosting;

        public Battle(BattleStaticData battleStaticData)
        {
            BattleStaticData = battleStaticData;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(battleStaticData.SceneName);

            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

            CrateManager = new(this);
            EffectManager = new(this);

            CrateManager = new(this);
            EnvironmentalEffectsManager = new(sceneConfiguration.EnvironmentalEffects);
            CloudManager = new(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager = new(sceneConfiguration.Flow);
            PrecipitationManager = new(Scene.Size, sceneConfiguration.PrecipitationType);
        }

        public Battle(Dictionary<int, AvatarType> avatars, Dictionary<int, int> playerTeamRelations, Dictionary<int, AvatarProp> avatarProps, ModeType mode, string sceneName) {
            Hosting = true;

            var sceneConfiguration = ConfigurationManager.GetSceneConfiguration(sceneName);
            Scene = new Scene(sceneConfiguration);

            CameraManager.Initialize(Scene.Size);

            ItemManager = new(this);
            ModeManager = new(this, avatars.Keys.Select(key => new KeyValuePair<int, int>(key, playerTeamRelations[key])), mode);

            if (ModeType.Capture_the_Flag == mode)
            {
                Dictionary<int, Point> flagDictionary = [];

                var orderedTeamIndices = playerTeamRelations.Values.Distinct().Order();

                foreach (var flag in sceneConfiguration.Flags)
                {
                    var teamIndex = flag.TeamIndex == 0 ? orderedTeamIndices.First() : orderedTeamIndices.Last();

                    ItemManager.CreateNewItem(ItemType.Flag, new Vector2(flag.X, flag.Y), teamIndex);

                    flagDictionary.Add(teamIndex, new Point(flag.X, flag.Y));
                }

                ModeManager.InitializeCaptureTheFlag(flagDictionary);

                Scene.UpdateTeamIndexes(orderedTeamIndices.First(), orderedTeamIndices.Last());
            }
            else
            {
                Scene.TeamRectangles?.Clear();
            }

            Avatars = [];
            foreach (var index in avatars.Keys)
            {
                var crowned = false;

                if (avatarProps.TryGetValue(index, out AvatarProp value))
                {
                    var avatarProp = value;
                    if (avatarProp != null)
                    {
                        crowned = avatarProp.Crowned;
                    }
                }

                var avatar = new Avatar(index, ConfigurationManager.GetAvatarConfiguration(avatars[index]), this, crowned);

                avatar.SetStartingPosition(GetSpawnLocation(index));

                Avatars.Add(index, avatar);
            }

            if (ModeType.Tutorial == mode)
            {
                Avatars.Values.ElementAt(0).SetSpawnLocation(new Vector2(1350, 540));
            }

            BulletManager = new(Scene.CollisionBoxes.Where(box => box.Height > CollisionHelper.PassableYThreshold).ToList(), Scene.Size, this);

            CrateManager = new(this);
            EffectManager = new(this);

            EnvironmentalEffectsManager = new(sceneConfiguration.EnvironmentalEffects);

            CloudManager = new(sceneConfiguration.HighCloudsOnly, Scene.Size);
            FlowManager = new(sceneConfiguration.Flow);
            PrecipitationManager = new(Scene.Size, sceneConfiguration.PrecipitationType);

            Frame = CreateFrame();
            StatFrame = ModeManager.CreateStatFrameIfNewData();
            BattleStaticData = CreateBattleStaticData(sceneName);

            BattleUpdate = new BattleUpdate
            {
                StatFrame = StatFrame
            };
        }

        public void Update()
        {
            while (UpdateQueue.Count > 0)
            {
                ApplyUpdate(UpdateQueue.Dequeue());
            }

            CameraManager.UpdateFocusOffset(Mouse.GetState().Position.ToVector2());

            EffectManager?.UpdateEffects();

            Avatars?.ToList().ForEach(avatar =>
            {
                if (!avatar.Value.IsDead && !GameOver)
                {
                    InputManager.UpdateAvatar(avatar.Key, avatar.Value);
                }

                PhysicsManager.Update(avatar.Value, Scene);
                avatar.Value.Update();
            });

            BulletManager?.UpdateBullets(Avatars?.Values.Where(avatar => !avatar.IsDead).ToList());

            EnvironmentalEffectsManager.UpdateEffects();
            CloudManager.UpdateClouds();
            FlowManager.UpdateFlows();
            PrecipitationManager.UpdatePrecipitation();

            CrateManager?.UpdateCrates(Avatars?.Values);

            ItemManager?.UpdateItems(Scene);

            if (Avatars != null)
            {
                CameraManager.UpdateFocusPoint(Avatars[BattleManager.FocusPlayerIndex].Position);
            }

            if (ModeManager != null)
            {
                if (ModeType.King_of_the_Hill == ModeManager.Mode)
                {
                    ModeManager.UpdateKingOfTheHill((Rectangle)Scene.CapturePoint, Avatars.Values);
                }
                else if (ModeType.Capture_the_Flag == ModeManager.Mode)
                {
                    ModeManager.UpdateCaptureTheFlag(
                        Scene.TeamRectangles.Where(rec => rec.ReturnZone),
                        ItemManager.Items.Where(item => ItemType.Flag == item.Type)
                    );
                }

                if (!GameOver && ModeManager.GameOver)
                {
                    BattleManager.SetGameOver();
                }

                var newStatFrame = ModeManager.CreateStatFrameIfNewData();
                if (newStatFrame != null)
                {
                    StatFrame = newStatFrame;
                    BattleUpdate ??= new BattleUpdate();
                    BattleUpdate.StatFrame = StatFrame;
                }
            }

            if (!Hosting) return;

            Frame = CreateFrame();
            if (SoundManager.HasAny())
            {
                if (ServerManager.IsServing)
                {
                    BattleUpdate ??= new BattleUpdate();
                    SoundManager.PushSounds(BattleUpdate);
                }
                else
                {
                    SoundManager.PlaySounds();
                }
            }

            if (ServerManager.IsServing)
            {
                ServerManager.SendBattleFrame(Frame);

                if (BattleUpdate != null)
                {
                    ServerManager.SendBattleUpdate(BattleUpdate);
                }
            }

            BattleUpdate = null;
        }

        public void ApplyUpdate(BattleUpdate battleUpdate)
        {
            if (battleUpdate.StatFrame != null)
            {
                StatFrame = battleUpdate.StatFrame;
            }

            var crateUpdate = battleUpdate.CrateUpdate;
            if (crateUpdate != null)
            {
                if (crateUpdate.NewTypes != null)
                {
                    for (int i = 0; i < crateUpdate.NewTypes.Count; i++)
                    {
                        CrateManager?.CreateNewCrate(
                            crateUpdate.NewTypes[i],
                            crateUpdate.NewXs[i],
                            crateUpdate.NewFinalYs[i],
                            crateUpdate.NewGoingDowns[i]
                        );
                    }
                }

                CrateManager?.DeleteCrates(crateUpdate.DeletedIds);
            }

            var effectUpdate = battleUpdate.EffectUpdate;
            if (effectUpdate != null)
            {
                if (effectUpdate.NewTypes != null)
                {
                    for (int i = 0; i < effectUpdate.NewTypes.Count; i++)
                    {
                        EffectManager?.CreateEffect(
                            new Vector2(effectUpdate.NewXs[i], effectUpdate.NewYs[i]),
                            effectUpdate.NewTypes[i],
                            direction: effectUpdate.NewDirectionLefts[i] ? Direction.Left : null,
                            fromUpdate: true
                        );
                    }
                }
            }

            SoundManager.PlaySounds(battleUpdate.SoundFrame);
        }

        public void Draw()
        {
            if (Frame != null)
            {
                var index = BattleStaticData.AvatarStaticData.PlayerIndices.IndexOf(BattleManager.FocusPlayerIndex);
                CameraManager.UpdateFocusPoint(new Vector2(Frame.AvatarFrame.Xs[index], Frame.AvatarFrame.Ys[index]));
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

            if (Frame != null && Frame.HudFrame != null && Frame.ModeFrame.Colors != null && Scene.CapturePoint != null)
            {
                var rectangle = new Rectangle(
                    Scene.CapturePoint.Value.X - CameraManager.Offset.X,
                    Scene.CapturePoint.Value.Y - CameraManager.Offset.Y,
                    Scene.CapturePoint.Value.Width,
                    Scene.CapturePoint.Value.Height
                );

                DrawingManager.DrawRectangle(rectangle, Frame.ModeFrame.Colors.ToColor());
            }

            if (Frame != null && Frame.HudFrame != null)
            {
                Scene.TeamRectangles.ForEach(rec =>
                {
                    var rectangle = new Rectangle(
                        rec.Rectangle.X - CameraManager.Offset.X,
                        rec.Rectangle.Y - CameraManager.Offset.Y,
                        rec.Rectangle.Width,
                        rec.Rectangle.Height
                    );

                    if (ModeManager != null)
                    {
                        DrawingManager.DrawRectangle(rectangle, DrawingHelper.GetTeamColor(rec.TeamIndex));
                    }
                    else
                    {
                        var orderedIndices = BattleStaticData.AvatarStaticData.TeamIndices.Order();
                        DrawingManager.DrawRectangle(rectangle, DrawingHelper.GetTeamColor(rec.TeamIndex == 0 ? orderedIndices.First() : orderedIndices.Last()));
                    }
                });

                DrawingManager.DrawCollection(AvatarDrawingHelper.GetDrawableAvatars(Frame.AvatarFrame, BattleStaticData.AvatarStaticData, BattleManager.PlayerIndex));
                DrawingManager.DrawCollection(CrateManager.GetDrawableCrates());
                DrawingManager.DrawCollection(ItemManager.GetDrawableItems(Frame.ItemFrame));
            }

            foreach (var list in Scene.TileLists.Where(list => list.Z > 0))
            {
                DrawingManager.DrawCollection([.. list.Tiles]);
            }

            if (Frame != null)
            {
                DrawingManager.DrawCollection(BulletManager.GetDrawableBullets(Frame.BulletFrame));
                DrawingManager.DrawCollection(EffectManager.GetDrawableEffects());
            }

            DrawingManager.DrawCollection(CloudManager.Clouds.Where(cloud => cloud.Foreground));
            DrawingManager.DrawCollection(PrecipitationManager.ForegroundParticles);

            if (Frame != null)
            {
                if (Frame.HudFrame.RespawnTimers != null)
                {
                    var offset = new Vector2(64, -48);
                    DrawingManager.DrawStrings(Frame.HudFrame.RespawnTimers, Frame.AvatarFrame.GetPositions().Select(position => position + offset - CameraManager.Offset.ToVector2()));
                }

                if (Frame.HudFrame != null)
                {
                    DrawingManager.DrawHud(BattleStaticData, Frame.HudFrame, Frame.ModeFrame, Frame.AvatarFrame, BattleManager.PlayerIndex);

                    if (Frame.ModeFrame.Colors != null && Scene.CapturePoint != null && Frame.ModeFrame.CapturePointSeconds != null)
                    {
                        var rectangle = new Rectangle(
                            Scene.CapturePoint.Value.X - CameraManager.Offset.X,
                            Scene.CapturePoint.Value.Y - CameraManager.Offset.Y,
                            Scene.CapturePoint.Value.Width,
                            Scene.CapturePoint.Value.Height
                        );

                        DrawingManager.DrawString(Frame.ModeFrame.CapturePointSeconds.ToString(), rectangle.Center.ToVector2(), DrawingHelper.MediumFont);
                    }

                    if (BattleStaticData.ModeType == ModeType.Capture_the_Flag)
                    {
                        var positions = new List<Vector2>();

                        if (Frame.HudFrame.FlagTimerXs != null)
                        {
                            for (int i = 0; i < Frame.HudFrame.FlagTimerXs.Count; i++)
                            {
                                positions.Add(
                                    new Vector2(Frame.HudFrame.FlagTimerXs[i], Frame.HudFrame.FlageTimerYs[i]) - CameraManager.Offset.ToVector2() + new Vector2(64, -16)
                                );
                            }
                        }

                        if (Frame.HudFrame.FlagTimerValues != null)
                        {
                            DrawingManager.DrawStrings(
                               Frame.HudFrame.FlagTimerValues.Select(value => value.ToString()),
                               positions
                           );
                        }
                    }

                    if (ControlsManager.IsControlDown(0, Control.Show_Stats) && ModeType.Tutorial != BattleStaticData.ModeType)
                    {
                        DrawingManager.DrawRectangle(new Rectangle(478, 268, 964, 544), Color.White);
                        DrawingManager.DrawRectangle(new Rectangle(480, 270, 960, 540), Color.Black);

                        var x = 208;

                        var startingY = 270 + 64;
                        var startingX = 480 + 64 + (x / 2);

                        var y = 58.85f;

                        DrawingManager.DrawString("K/D", new Vector2(startingX + 208, startingY), DrawingHelper.SmallFont);
                        DrawingManager.DrawString("Dmg Dealt", new Vector2(startingX + 208 * 2, startingY), DrawingHelper.SmallFont);
                        DrawingManager.DrawString("Dmg Taken", new Vector2(startingX + 208 * 3, startingY), DrawingHelper.SmallFont);

                        for (int i = 0; i < StatFrame.Names.Count; i++)
                        {
                            DrawingManager.DrawString(StatFrame.Names[i], new Vector2(startingX, startingY + ((i + 1) * y)), DrawingHelper.TinyFont,
                                DrawingHelper.GetTeamColor(BattleStaticData.AvatarStaticData.GetTeamIndex(StatFrame.PlayerIndices[i])));

                            DrawingManager.DrawString(StatFrame.Kills[i] + " / " + StatFrame.Deaths[i], new Vector2(startingX + x, startingY + ((i + 1) * y)), DrawingHelper.TinyFont);
                            DrawingManager.DrawString(StatFrame.DamageDealts[i].ToString(), new Vector2(startingX + x * 2, startingY + ((i + 1) * y)), DrawingHelper.TinyFont);
                            DrawingManager.DrawString(StatFrame.DamageTakens[i].ToString(), new Vector2(startingX + x * 3, startingY + ((i + 1) * y)), DrawingHelper.TinyFont);
                        }
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
                AvatarFrame = AvatarFrame.CreateFrom(Avatars),
                BulletFrame = BulletManager.GetBulletFrame(),
                HudFrame = CreateHudFrame(),
                ModeFrame = CreateModeFrame(),
                ItemFrame = ItemManager.GetItemFrame()
            };

            return battleFrame;
        }

        private BattleStaticData CreateBattleStaticData(string sceneName)
        {
            var relations = ModeManager.PlayerTeamRelations;
            var showColors = ModeType.Tutorial != Mode && (ModeType.Deathmatch != Mode || (relations.Values.Distinct().Count() != relations.Count));

            var battleStaticData = new BattleStaticData
            {
                SceneName = sceneName,
                ModeType = Mode,
                Names = ModeManager.PlayerTeamRelations.Keys.Select(ServerManager.GetPlayerName).ToList(),
                AvatarStaticData = AvatarStaticData.CreateFrom(Avatars, relations, showColors, showCrowns: ModeType.Regicide == Mode),
            };

            return battleStaticData;
        }

        private HudFrame CreateHudFrame()
        {
            var hudFrame = new HudFrame();

            if (ModeType.Capture_the_Flag == Mode)
            {
                hudFrame.FlagTimerValues = [];
                hudFrame.FlagTimerXs = [];
                hudFrame.FlageTimerYs = [];

                foreach (var timer in ModeManager.FlagReturnTimers)
                {
                    if (timer.Value != 0)
                    {
                        hudFrame.FlagTimerValues.Add(5 - (timer.Value / 60));

                        var flag = ItemManager.Items.Single(item => item.TeamIndex == timer.Key && item.Type == ItemType.Flag);

                        hudFrame.FlagTimerXs.Add(flag.Position.X);
                        hudFrame.FlageTimerYs.Add(flag.Position.Y);
                    }
                }
            }

            if (Avatars.Values.Any(avatar => avatar.RespawnTimerFrames > 0))
            {
                hudFrame.RespawnTimers = [];
                foreach (var avatar in Avatars.Values)
                {
                    hudFrame.RespawnTimers.Add(avatar.GetRespawnMessage());
                }
            }

            foreach (var avatarKey in Avatars.Keys)
            {
                var avatar = Avatars[avatarKey];
                var weapon = avatar.SelectedWeapon;

                hudFrame.Healths.Add(avatar.Health);
                hudFrame.Ammos.Add(weapon.AmmoInClip + weapon.Ammo);
            }

            return hudFrame;
        }

        private ModeUpdate CreateModeFrame()
        {
            var modeFrame = new ModeUpdate();

            if (ModeType.Tutorial != Mode)
            {
                modeFrame.ModeValues = ModeManager.GetModeValues();
            }

            if (ModeType.King_of_the_Hill == Mode)
            {
                modeFrame.Colors = ColorJson.CreateFrom(ModeManager.GetCapturePointColor());
                modeFrame.CapturePointSeconds = ModeManager.CapturePointSeconds;
            }

            return modeFrame;
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

        public bool IsAlive(int playerIndex)
        {
            if (Avatars != null && Avatars.TryGetValue(playerIndex, out Avatar value))
            {
                return !value.IsDead;
            }
            else if (Frame != null)
            {
                var index = BattleStaticData.AvatarStaticData.PlayerIndices.IndexOf(playerIndex);

                return Frame.AvatarFrame.Animations[index] != Animation.Dead;
            }

            return true;
        }

        public int GetNextPlayerIndex(int focusPlayerIndex)
        {
            List<int> playerIds = [];
            if (Avatars != null)
            {
                playerIds = [.. Avatars.Keys];
            }
            else if (Frame != null)
            {
                playerIds = [.. BattleStaticData.AvatarStaticData.PlayerIndices];
            }

            if (playerIds.Count != 0)
            {
                var index = playerIds.IndexOf(focusPlayerIndex);
                index++;
                if (index > playerIds.Count - 1)
                {
                    index = 0;
                }

                return playerIds[index];
            }

            return focusPlayerIndex;
        }

        public Vector2 GetSpawnLocation(int playerIndex)
        {
            bool? leftTeam = null;
            if (ModeType.Capture_the_Flag == Mode)
            {
                var teamIndices = ModeManager.PlayerTeamRelations.Values.Distinct().Order();
                leftTeam = ModeManager.PlayerTeamRelations[playerIndex] == teamIndices.First();
            }

            var index = SpawnLocationIndex++;

            if (SpawnLocationIndex > 5)
            {
                SpawnLocationIndex = 0;
            }

            if (leftTeam != null)
            {
                var newIndex = index > 2 ? index - 3 : index;

                return Scene.StartingPositions[leftTeam == true ? 0 : 1] + new Vector2(newIndex * 32f - 32f, 0);
            }

            return Scene.StartingPositions[index];
        }

        public bool BeingHeld(Item item)
        {
            return Avatars.Values.Any(avatar => avatar.HeldItems.Contains(item));
        }

        public ICollection<Rectangle> GetCollisionBoxes()
        {
            return Scene.CollisionBoxes;
        }

        public Point GetSceneSize()
        {
            return Scene.Size;
        }

        public void BulletCreated(Bullet bullet)
        {
            throw new NotImplementedException();
        }

        public void BulletDeleted(int id)
        {
            throw new NotImplementedException();
        }

        public void CrateCreated(Crate crate)
        {
            BattleUpdate ??= new BattleUpdate();

            BattleUpdate.CrateCreated(crate);
        }

        public void CrateDeleted(int id)
        {
            BattleUpdate ??= new BattleUpdate();

            BattleUpdate.CrateDeleted(id);
        }

        public void EffectCreated(Effect effect)
        {
            BattleUpdate ??= new BattleUpdate();

            BattleUpdate.EffectCreated(effect);
        }
    }
}
