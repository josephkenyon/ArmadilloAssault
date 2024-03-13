using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Scenes;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.Assets
{
    public class Scene(SceneJson json) : IPhysicsScene
    {
        public TextureName BackgroundTexture { get; set; } = json.BackgroundTexture;
        public TextureName TilesetTexture { get; set; } = json.TilesetTexture;
        public Point Size { get; private set; } = json.Size != null ? json.Size.ToPoint() : new Point(1920, 1080);
        public ColorJson BackgroundColorJson { get; private set; } = json.BackgroundColor;
        public Color BackgroundColor { get; private set; } = json.BackgroundColor != null ? new Color(json.BackgroundColor.R, json.BackgroundColor.G, json.BackgroundColor.B) : Color.CornflowerBlue;
        public List<Rectangle> CollisionBoxes { get; set; } = ConfigurationHelper.GetRectangles(json.CollisionBoxes);
        public List<TileList> TileLists { get; set; } = GetTileLists(json);
        public bool WrapY { get; set; } = json.WrapY;
        public Rectangle? CapturePoint { get; set; } = json.CapturePoint?.ToRectangle(DrawingHelper.FullTileSize);

        public readonly List<Vector2> StartingPositions = json.StartingPositions.Select(position => (position.ToVector2() * DrawingHelper.FullTileSize)).ToList();

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
            tile.Texture = textureName;
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

        private static List<TileList> GetTileLists(SceneJson json)
        {
            var tileLists = new List<TileList>();

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
                        Texture = json.TilesetTexture,
                        Z = tileList.Z
                    };

                    tileList.Tiles.Add(newTile);
                }

                tileLists.Add(tileList);
            }

            return [.. tileLists.OrderBy(list => list.Z)];
        }

        public ICollection<Rectangle> GetCollisionBoxes() => CollisionBoxes;

        public Point GetSize() => Size;

        public bool YWraps() => WrapY;
    }
}