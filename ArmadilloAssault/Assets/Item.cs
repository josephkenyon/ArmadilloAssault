using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Assets
{
    public class Item(IItemListener itemListener, ItemType itemType, int teamIndex) : PhysicsObject
    {
        public readonly int TeamIndex = teamIndex;
        public readonly ItemType Type = itemType;

        public override Rectangle GetCollisionBox()
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

        public bool BeingHeld => itemListener.BeingHeld(this);
        public bool CanBePickedUp => !BeingHeld;
    }
}
