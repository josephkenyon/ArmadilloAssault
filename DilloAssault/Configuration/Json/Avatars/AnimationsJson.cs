using DilloAssault.Configuration.Json.Scenes;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Json.Avatars
{
    public class AnimationsJson
    {
        public AnimationJson Running { get; set; }
        public AnimationJson Jumping { get; set; }
        public AnimationJson Spinning { get; set; }
        public AnimationJson Falling { get; set; }
    }
}
