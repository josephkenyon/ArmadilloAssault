using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Assets
{
    public class Item(IItemListener itemListener)
    {
        public ItemType Type { get; set; }
        public Vector2 Position { get; set; }
        public Direction Direction { get; set; }
        public Rectangle GetCollisionBox()
        {
            var jsonRectangle = ConfigurationManager.GetItemConfiguration(Type).CollisionRectangle;
            var position = Position.ToPoint();

            return new Rectangle(
                position.X + jsonRectangle.X,
                position.Y + jsonRectangle.Y,
                jsonRectangle.Width,
                jsonRectangle.Height
            );
        }

        public int? TeamIndex => itemListener.TeamHeldIndex(this);
        public bool BeingHeld => itemListener.BeingHeld(this);
        public bool CanBePickedUp => !BeingHeld;
    }
}
