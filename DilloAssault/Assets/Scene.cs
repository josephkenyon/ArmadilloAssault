using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Configuration.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArmadilloAssault.Assets
{
    public class Scene(SceneJson json)
    {
        public TextureName BackgroundTexture { get; set; } = Enum.Parse<TextureName>(json.BackgroundTexture);
        public List<Rectangle> CollisionBoxes { get; set; } = ConfigurationHelper.GetHurtBoxes(json.CollisionBoxes);
        public List<TileList> TileLists { get; set; } = GetTileLists(json.TileLists);

        public void UpdateTile(int z, Point position, Point spriteLocation, TextureName textureName)
        {
            var tileList = TileLists.SingleOrDefault(list => list.Z == z);

            if (tileList == null)
            {
                tileList = new TileList
                {
                    Z = z
                };

                TileLists.Add(tileList);

                TileLists = [.. TileLists.OrderBy(list => list.Z)];
            }

            tileList.Tiles.RemoveAll(tile => tile.Position.Equals(position));

            var tile = new Tile
            {
                Position = position
            };

            tileList.Tiles.Add(tile);

            tile.SpriteLocation = spriteLocation;
            tile.TextureName = textureName;
            tile.Z = z;
        }

        public void DeleteTile(int z, Point position)
        {
            var tileList = TileLists.SingleOrDefault(list => list.Z == z);

            if (tileList != null)
            {
                var tiles = tileList.Tiles.Where(tile => tile.Position.Equals(position));

                tileList.Tiles.RemoveAll(tile => tile.Position.Equals(position));

                if (tileList.Tiles.Count == 0)
                {
                    TileLists.Remove(tileList);
                }
            }
        }

        public void DeleteCollisionBox(Vector2 cursorPosition)
        {
            CollisionBoxes.RemoveAll(rectangle => rectangle.Contains(cursorPosition));
        }

        public void UpdateCollisionBox(Vector2 newEndPoint)
        {
            var index = CollisionBoxes.Count - 1;

            var collisionBox = CollisionBoxes[index];

            var newSize = newEndPoint - CollisionBoxes[index].Location.ToVector2();

            if (newSize.X > 0 && newSize.Y > 0)
            {
                collisionBox.Width = (int)newSize.X;
                collisionBox.Height = (int)newSize.Y;
            }

            CollisionBoxes.RemoveAt(index);
            CollisionBoxes.Add(collisionBox);
        }

        private static List<TileList> GetTileLists(List<TileListJson> tileListJsons)
        {
            Collection<TileList> tileLists = [];

            foreach (var jsonTileList in tileListJsons)
            {
                var tileList = new TileList
                {
                    Z = jsonTileList.Z
                };

                for (var i = 0; i < jsonTileList.X.Count; i++)
                {
                    var newTile = new Tile
                    {
                        Position = new Point(jsonTileList.X[i], jsonTileList.Y[i]),
                        SpriteLocation = new Point(jsonTileList.SpriteX[i], jsonTileList.SpriteY[i]),
                        TextureName = Enum.Parse<TextureName>(jsonTileList.Texture),
                        Z = tileList.Z
                    };

                    tileList.Tiles.Add(newTile);
                }

                tileLists.Add(tileList);
            }

            return [.. tileLists.OrderBy(list => list.Z)];
        }
    }
}