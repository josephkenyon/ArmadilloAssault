using DilloAssault.Graphics.Drawing.Textures;

namespace DilloAssault.Configuration.Json.Avatars
{
    public class AvatarJson
    {
        public TextureName SpriteTextureName { get; set; }
        public TextureName ArmTextureName { get; set; }
        public PointJson ArmOrigin { get; set; }
        public AnimationsJson Animations { get; set; }
        public PointJson Size { get; set; }
        public RectangleJson CollisionBox { get; set; }
        public HurtBoxListJson HurtBoxes { get; set; }
        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }
    }
}
