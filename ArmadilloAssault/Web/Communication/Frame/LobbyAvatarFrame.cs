using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class LobbyAvatarFrame
    {
        [JsonProperty("Ts")]
        public List<AvatarType> Types { get; set; } = [];

        [JsonProperty("Ds")]
        public List<Direction> Directions { get; set; } = [];

        [JsonProperty("Cs")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> Crowneds { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Xs { get; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatConverter))]
        public List<float> Ys { get; } = [];

        public static LobbyAvatarFrame CreateFrom(Dictionary<int, Avatar> avatars, int yOffset, bool showCrowns)
        {
            var avatarFrame = new LobbyAvatarFrame();
            foreach (var playerIndex in avatars.Keys)
            {
                var avatar = avatars[playerIndex];

                avatarFrame.Types.Add(avatar.Type);
                avatarFrame.Directions.Add(avatar.Direction);
                avatarFrame.Crowneds.Add(avatar.Crowned && showCrowns);
                avatarFrame.Xs.Add(avatar.Position.X);
                avatarFrame.Ys.Add(avatar.Position.Y + yOffset);
            }

            return avatarFrame;
        }

        public List<Vector2> GetPositions()
        {
            var positions = new List<Vector2>();

            for (int i = 0; i < Xs.Count; i++)
            {
                positions.Add(new Vector2(Xs[i], Ys[i]));
            }

            return positions;
        }
    }
}
