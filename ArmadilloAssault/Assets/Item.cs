using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

        public bool BeingHeld { get; set; }
        public bool CanBePickedUp => itemListener.CanBePickedUp(this);
    }
}
