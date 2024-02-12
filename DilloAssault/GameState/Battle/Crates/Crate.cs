using DilloAssault.Configuration.Textures;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type) : PhysicsObject, IDrawableObject
    {
        private static readonly Random Random = new();

        public static readonly int Size = 64;
        public TextureName TextureName => TextureName.crates;
        public CrateType Type { get; private set; } = type;

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? GetRandomWeaponType() : null;

        public override Rectangle GetCollisionBox() => new((int)Position.X + 2, (int)Position.Y + 8, 60, 48);
        public Rectangle? GetSourceRectangle() => new((int)Type * Size, 0, Size, Size);
        public Rectangle GetDestinationRectangle() => new((int)Position.X, (int)Position.Y, Size, Size);
        public List<Rectangle> RelevantCollisionBoxes { get; set; } = [];


        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }
    }
}
