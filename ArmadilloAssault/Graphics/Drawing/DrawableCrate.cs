using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Battle.Crates;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableCrate(CrateType type, Vector2 position, bool grounded, bool goingDown) : IDrawableObject
    {
        public Point Size => grounded ? new Point(96, 96) : new Point(128, 204);
        public TextureName Texture => grounded ? TextureName.crates : (goingDown ? TextureName.crates_parachuting : TextureName.crates_ballooning);

        public Rectangle? GetSourceRectangle() => new((int)type * Size.X, 0, Size.X, Size.Y);
        public Rectangle GetDestinationRectangle() => new(
            (int)position.X - (!grounded ? 14 : 0) - CameraManager.Offset.X,
            (int)position.Y - (!grounded ? 109 : 0) - CameraManager.Offset.Y,
            Size.X, Size.Y
        );
    }
}
