using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Generics;
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

        public void SetDirection(Direction direction)
        {
            Direction = direction;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public bool Disturbed { get; set; }
        public bool BeingHeld => itemListener.BeingHeld(this);
        public bool CanBePickedUp => !BeingHeld;
    }
}
