using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.PowerUps;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class AvatarFrame
    {
        [JsonProperty("As")]
        public List<Animation> Animations { get; set; } = [];

        [JsonProperty("AAs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> ArmAngles { get; set; } = [];

        [JsonProperty("Fs")]
        public List<int> AnimationFrames { get; set; } = [];

        [JsonProperty("BOs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> BreathingYOffsets { get; set; } = [];

        [JsonProperty("Ds")]
        public List<Direction> Directions { get; set; } = [];

        [JsonProperty("Xs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Xs { get; } = [];

        [JsonProperty("Ys")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Ys { get; } = [];

        [JsonProperty("Rcs")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Recoils { get; set; } = [];

        [JsonProperty("Rts")]
        [JsonConverter(typeof(FloatListConverter))]
        public List<float> Rotations { get; } = [];

        [JsonProperty("Is")]
        [JsonConverter(typeof(BooleanConverter))]
        public List<bool> Invisibles { get; set; } = [];

        [JsonProperty("Ts")]
        public List<TextureName> WeaponTextures { get; set; } = [];

        [JsonProperty("Cs", NullValueHandling = NullValueHandling.Ignore)]
        public List<ColorJson> Colors { get; set; }

        public static AvatarFrame CreateFrom(Dictionary<int, Avatar> avatars)
        {
            var avatarFrame = new AvatarFrame();

            foreach (var playerIndex in avatars.Keys)
            {
                var avatar = avatars[playerIndex];

                avatarFrame.Animations.Add(avatar.Animation);
                avatarFrame.ArmAngles.Add((float)avatar.ArmAngle);
                avatarFrame.AnimationFrames.Add(avatar.AnimationFrame);
                avatarFrame.BreathingYOffsets.Add(avatar.GetBreathingYOffset());
                avatarFrame.Directions.Add(avatar.Direction);
                avatarFrame.Xs.Add(avatar.Position.X);
                avatarFrame.Ys.Add(avatar.Position.Y);
                avatarFrame.Recoils.Add(avatar.GetRecoil);
                avatarFrame.Rotations.Add(avatar.Rotation);
                avatarFrame.Invisibles.Add(avatar.CurrentPowerUp == PowerUpType.Invisibility);
                avatarFrame.WeaponTextures.Add(avatar.CurrentWeaponConfiguration.TextureName);
            }

            var colors = avatars.Values.Select(avatar => avatar.GetColor());
            if (colors.Any(color => color != null))
            {
                avatarFrame.Colors = colors.ToList();
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
