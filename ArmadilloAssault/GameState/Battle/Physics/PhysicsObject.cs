﻿using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Physics
{
    public abstract class PhysicsObject
    {
        public Direction Direction { get; protected set; }
        public Vector2? SpawnLocation { get; protected set; }
        public Vector2 Position { get; protected set; } = Vector2.Zero;
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public virtual Vector2 MaxVelocity => new(8f, 11);

        public virtual float RunningAcceleration => 0.65f;
        public float RunningVelocity { get; set; }
        public int InfluenceVelocity { get; set; }

        public bool CloseToGround { get; set; }
        public bool Grounded { get; set; }

        public bool Falling { get; set; }
        public bool Rising { get; set; }
        public virtual float DragModifier => 1f;

        public void SetX(float x)
        {
            Position = new Vector2(x, Position.Y);
        }

        public void SetY(float y)
        {
            Position = new Vector2(Position.X, y);
        }

        public void SetSpawnLocation(Vector2 position)
        {
            SpawnLocation = position;
            Position = position;
        }

        public abstract Rectangle GetCollisionBox();
    }
}
