using ArmadilloAssault.GameState.Battle.Bullets;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Avatars
{
    public interface IAvatarListener : IWeaponListener
    {
        void AvatarHit(int hitIndex, int firedIndex, int damage);
        void AvatarKilled(int deadIndex, int? killIndex);
        Vector2 GetSpawnLocation();
    }
}
