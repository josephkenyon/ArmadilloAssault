using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Configuration.Textures;

namespace ArmadilloAssault.Configuration.Menus
{
    public class ItemJson
    {
        public ItemType Type { get; set; }
        public TextureName TextureName { get; set; }
        public RectangleJson CollisionRectangle { get; set; }
    }
}
