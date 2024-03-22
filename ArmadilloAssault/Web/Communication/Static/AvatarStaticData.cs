using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Textures;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class AvatarStaticData
    {
        public List<int> PlayerIndices { get; set; } = [];
        public List<int> TeamIndices { get; set; } = [];
        public List<bool> ShowTeamColors { get; set; } = [];
        public List<bool> HasCrowns { get; set; } = [];
        public List<TextureName> TextureNames { get; set; } = [];
        public List<TextureName> WhiteTextureNames { get; set; } = [];
        public List<AvatarType> Types { get; set; } = [];

        public static AvatarStaticData CreateFrom(Dictionary<int, Avatar> avatars, Dictionary<int, int> playerTeamRelations, bool showColors, bool showCrowns = false)
        {
            var avatarFrame = new AvatarStaticData();
            foreach (var playerIndex in avatars.Keys)
            {
                var avatar = avatars[playerIndex];

                avatarFrame.PlayerIndices.Add(playerIndex);
                avatarFrame.TeamIndices.Add(playerTeamRelations[playerIndex]);
                avatarFrame.ShowTeamColors.Add(showColors);
                avatarFrame.TextureNames.Add(avatar.TextureName);
                avatarFrame.WhiteTextureNames.Add(avatar.WhiteTextureName);
                avatarFrame.Types.Add(avatar.Type);
                avatarFrame.HasCrowns.Add(avatar.Crowned && showCrowns);
            }

            return avatarFrame;
        }
    }
}
