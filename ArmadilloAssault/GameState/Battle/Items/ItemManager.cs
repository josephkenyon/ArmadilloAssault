using ArmadilloAssault.Assets;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public class ItemManager(IItemListener itemListener)
    {
        public List<Item> Items = [];

        public ItemFrame GetItemFrame()
        {
            if (Items.Count == 0) return null;

            var frame = new ItemFrame();

            foreach (var item in Items)
            {
                frame.ItemTypes.Add(item.Type);
                frame.Directions.Add(item.Direction);
                frame.SpriteXs.Add(item.BeingHeld ? 1 : 0);
                frame.PositionXs.Add(item.Position.X);
                frame.PositionYs.Add(item.Position.Y);
            }

            return frame;
        }

        public static List<IDrawableObject> GetDrawableItems(ItemFrame itemFrame)
        {
            var drawableItemList = new List<IDrawableObject>();

            if (itemFrame == null)
            {
                return drawableItemList;
            }

            var index = 0;
            foreach (var type in itemFrame.ItemTypes)
            {
                drawableItemList.Add(new DrawableItem(
                    type,
                    new Vector2(
                        itemFrame.PositionXs[index],
                        itemFrame.PositionYs[index]
                    ),
                    itemFrame.Directions[index],
                    itemFrame.SpriteXs[index]
                ));

                index++;
            }

            return drawableItemList;
        }
    }
}
