﻿using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class AvatarFrame
    {
        public List<int> PlayerIndices { get; set; } = [];
        public List<Animation> Animations { get; set; } = [];
        public List<float> ArmAngles { get; set; } = [];
        public List<int> AnimationFrames { get; set; } = [];
        public List<string> RespawnTimers { get; set; } = [];
        public List<float> BreathingYOffsets { get; set; } = [];
        public List<bool> Deads { get; set; } = [];
        public List<Direction> Directions { get; set; } = [];
        public List<Vector2> Positions { get; } = [];
        public List<float> Recoils { get; set; } = [];
        public List<float> Rotations { get; } = [];
        public List<bool> Spinnings { get; set; } = [];
        public List<TextureName> TextureNames { get; set; } = [];
        public List<AvatarType> Types { get; set; } = [];
        public List<TextureName> WeaponTextures { get; set; } = [];
        public List<ColorJson> Colors { get; set; } = [];

        public static AvatarFrame CreateFrom(Dictionary<PlayerIndex, Avatar> avatars)
        {
            var avatarFrame = new AvatarFrame();
            foreach (var playerIndex in avatars.Keys)
            {
                var avatar = avatars[playerIndex];

                avatarFrame.PlayerIndices.Add((int)playerIndex);
                avatarFrame.Animations.Add(avatar.Animation);
                avatarFrame.ArmAngles.Add((float)avatar.ArmAngle);
                avatarFrame.AnimationFrames.Add(avatar.AnimationFrame);
                avatarFrame.RespawnTimers.Add(avatar.GetRespawnMessage());
                avatarFrame.BreathingYOffsets.Add(avatar.GetBreathingYOffset());
                avatarFrame.Deads.Add(avatar.IsDead);
                avatarFrame.Directions.Add(avatar.Direction);
                avatarFrame.Positions.Add(avatar.Position);
                avatarFrame.Recoils.Add(avatar.GetRecoil);
                avatarFrame.Rotations.Add(avatar.Rotation);
                avatarFrame.Spinnings.Add(avatar.IsSpinning);
                avatarFrame.TextureNames.Add(avatar.TextureName);
                avatarFrame.Types.Add(avatar.Type);
                avatarFrame.WeaponTextures.Add(avatar.CurrentWeaponConfiguration.TextureName);
                avatarFrame.Colors.Add(avatar.GetColor());
            }

            return avatarFrame;
        }
    }
}
