using DilloAssault.Configuration.Json.Scenes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Configuration.Json.Avatars
{
    public class AvatarJson
    {
        public RectangleJson CollisionBox { get; set; }
        public HurtBoxListJson HurtBoxes { get; set; }
        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }
    }
}
