using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.PowerUps;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class AvatarFrame
    {
        public List<int> PlayerIndices { get; set; } = [];
        public List<int> TeamIndices { get; set; } = [];
        public List<bool> ShowTeamColors { get; set; } = [];
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
        public List<bool> Invisibles { get; set; } = [];
        public List<bool> HasCrowns { get; set; } = [];
        public List<TextureName> TextureNames { get; set; } = [];
        public List<TextureName> WhiteTextureNames { get; set; } = [];
        public List<AvatarType> Types { get; set; } = [];
        public List<TextureName> WeaponTextures { get; set; } = [];
        public List<ColorJson> Colors { get; set; } = [];

        public static AvatarFrame CreateFrom(Dictionary<int, Avatar> avatars, Dictionary<int, int> playerTeamRelations, bool showColors, int yOffset = 0, bool showCrowns = false)
        {
            var avatarFrame = new AvatarFrame();
            foreach (var playerIndex in avatars.Keys)
            {
                var avatar = avatars[playerIndex];

                avatarFrame.PlayerIndices.Add(playerIndex);
                avatarFrame.TeamIndices.Add(playerTeamRelations[playerIndex]);
                avatarFrame.ShowTeamColors.Add(showColors);
                avatarFrame.Animations.Add(avatar.Animation);
                avatarFrame.ArmAngles.Add((float)avatar.ArmAngle);
                avatarFrame.AnimationFrames.Add(avatar.AnimationFrame);
                avatarFrame.RespawnTimers.Add(avatar.GetRespawnMessage());
                avatarFrame.BreathingYOffsets.Add(avatar.GetBreathingYOffset());
                avatarFrame.Deads.Add(avatar.IsDead);
                avatarFrame.Directions.Add(avatar.Direction);
                avatarFrame.Positions.Add(new Vector2(avatar.Position.X, avatar.Position.Y + yOffset));
                avatarFrame.Recoils.Add(avatar.GetRecoil);
                avatarFrame.Rotations.Add(avatar.Rotation);
                avatarFrame.Spinnings.Add(avatar.IsSpinning);
                avatarFrame.Invisibles.Add(avatar.CurrentPowerUp == PowerUpType.Invisibility);
                avatarFrame.TextureNames.Add(avatar.TextureName);
                avatarFrame.WhiteTextureNames.Add(avatar.WhiteTextureName);
                avatarFrame.Types.Add(avatar.Type);
                avatarFrame.WeaponTextures.Add(avatar.CurrentWeaponConfiguration.TextureName);
                avatarFrame.HasCrowns.Add(avatar.Crowned && showCrowns);
                avatarFrame.Colors.Add(avatar.GetColor());
            }

            return avatarFrame;
        }
    }
}
