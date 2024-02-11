using DilloAssault.Configuration.Textures;
using DilloAssault.Configuration.Weapons;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type) : IDrawableObject
    {
        private static readonly Random Random = new();

        private readonly int CrateSize = 64;
        public TextureName TextureName => TextureName.crates;
        public CrateType Type { get; private set; } = type;
        public Vector2 Position { get; set; }

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? GetRandomWeaponType() : null;

        public Rectangle GetCollisionBox() => new((int)Position.X + 2 * CrateSize, (int)Position.X + 8, 60, 48);
        public Rectangle GetSourceRectangle() => new((int)Type * CrateSize, 0, CrateSize, CrateSize);
        public Rectangle GetDestinationRectangle() => new((int)Position.X, (int)Position.Y, CrateSize, CrateSize);

        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }
    }
}
