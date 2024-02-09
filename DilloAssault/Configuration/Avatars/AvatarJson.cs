using DilloAssault.Configuration.Generics;
using DilloAssault.Configuration.Textures;

namespace DilloAssault.Configuration.Avatars
{
    public class AvatarJson
    {
        public TextureName TextureName { get; set; }
        public PointJson ArmOrigin { get; set; }
        public PointJson HeadOrigin { get; set; }
        public AnimationsJson Animations { get; set; }
        public PointJson Size { get; set; }
        public PointJson SpriteOffset { get; set; }
        public RectangleJson CollisionBox { get; set; }
        public RectangleJson SpinningCollisionBox { get; set; }
        public HurtBoxListJson HurtBoxes { get; set; }
        public RectangleJson ShellBox { get; set; }
        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }
    }
}
