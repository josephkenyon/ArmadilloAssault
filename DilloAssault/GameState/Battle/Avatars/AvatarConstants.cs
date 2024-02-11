using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Avatars
{
    public static class AvatarConstants
    {
        public static readonly int spriteWidth = 128;
        public static readonly int spriteHeight = 128;

        public static readonly Vector2 MaxVelocity = new(8f, 11);

        public static readonly float RunningAcceleration = 0.65f;
        public static readonly float JumpingAcceleration = 0.5f;
        public static readonly float MaxRunningVelocity = 6.5f;

        public static readonly int BreathingCycleFrameLength = 80;
    }
}
