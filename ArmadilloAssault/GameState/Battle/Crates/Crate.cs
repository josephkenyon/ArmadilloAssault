﻿using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type) : PhysicsObject
    {
        private static readonly Random Random = new();

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? GetRandomWeaponType() : null;

        public Point Size => Grounded ? new Point(96, 96) : new Point(128, 204);
        public TextureName TextureName => Grounded ? TextureName.crates : TextureName.crates_parachuting;
        public CrateType Type { get; private set; } = type;
        public List<Rectangle> RelevantCollisionBoxes { get; set; } = [];
        public override Rectangle GetCollisionBox() => new((int)Position.X + 18, (int)Position.Y + 24, 60, 52);
        public override Vector2 MaxVelocity => new(8f, 2);

        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }
    }
}
