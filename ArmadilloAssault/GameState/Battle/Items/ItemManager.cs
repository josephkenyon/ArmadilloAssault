﻿using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.GameState.Battle.Items;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public class ItemManager(IItemListener itemListener)
    {
        public List<Item> Items = [];

        public void CreateNewItem(ItemType type, Vector2 position, int teamIndex)
        {
            var item = new Item(itemListener, type, teamIndex);

            item.SetSpawnLocation(position);

            Items.Add(item);
        }

        public void UpdateItems(IPhysicsScene physicsScene)
        {
            foreach (var item in Items.Where(i => !i.BeingHeld))
            {
                PhysicsManager.Update(item, physicsScene);

                var itemBox = item.GetCollisionBox();

                var avatar = itemListener.GetAvatars().Values
                    .FirstOrDefault(avatar => !avatar.IsDead && avatar.TeamIndex != item.TeamIndex && avatar.GetCollisionBox().Intersects(itemBox));

                if (avatar != null)
                {
                    item.Disturbed = true;
                    avatar.GiveItem(item);
                }
            }
        }

        public ItemFrame GetItemFrame()
        {
            if (Items.Count == 0) return null;

            var frame = new ItemFrame();

            foreach (var item in Items)
            {
                frame.Types.Add(item.Type);
                frame.Directions.Add(item.Direction);
                frame.TeamIndices.Add(item.TeamIndex);
                frame.SpriteXs.Add(item.BeingHeld ? 1 : 0);
                frame.Xs.Add(item.Position.X);
                frame.Ys.Add(item.Position.Y);
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
            foreach (var type in itemFrame.Types)
            {
                Color? color = itemFrame.TeamIndices[index] != null ? DrawingHelper.GetTeamColor((int)itemFrame.TeamIndices[index]) : null;

                var drawableItem = new DrawableItem(
                    type,
                    new Vector2(
                        itemFrame.Xs[index],
                        itemFrame.Ys[index]
                    ),
                    itemFrame.Directions[index],
                    itemFrame.SpriteXs[index],
                    color
                );

                drawableItemList.Add(drawableItem);

                index++;
            }

            return drawableItemList;
        }
    }
}
