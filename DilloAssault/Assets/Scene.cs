using DilloAssault.Configuration.Scenes;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DilloAssault.Assets
{
    public class Scene(SceneJson json)
    {
        public List<TileList> TileLists { get; set; } = GetTileLists(json);

        public void UpdateTile(int z, Point position, Point spriteLocation, TextureName textureName)
        {
            var tileList = TileLists.SingleOrDefault(list => list.Z == z);

            if (tileList == null)
            {
                tileList = new TileList();
                TileLists.Add(tileList);
            }

            tileList.Z = z;

            var tile = tileList.Tiles.SingleOrDefault(tile => tile.Position.Equals(position));

            if (tile == null)
            {
                tile = new Tile
                {
                    Position = position
                };

                tileList.Tiles.Add(tile);
            }

            tile.SpriteLocation = spriteLocation;
            tile.TextureName = textureName;
            tile.Z = z;

            TileLists = [.. TileLists.OrderBy(list => list.Z)];
        }

        public void DeleteTile(int z, Point position)
        {
            var tileList = TileLists.SingleOrDefault(list => list.Z == z);

            if (tileList != null)
            {
                var tile = tileList.Tiles.SingleOrDefault(tile => tile.Position.Equals(position));

                if (tile != null)
                {
                    tileList.Tiles.Remove(tile);
                }

                if (tileList.Tiles.Count == 0)
                {
                    TileLists.Remove(tileList);
                }
            }
        }

        private static List<TileList> GetTileLists(SceneJson json)
        {
            Collection<TileList> tileLists = [];

            foreach (var jsonTileList in json.TileLists)
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