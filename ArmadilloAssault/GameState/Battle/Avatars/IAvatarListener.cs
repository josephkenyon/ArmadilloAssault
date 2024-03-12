using ArmadilloAssault.GameState.Battle.Bullets;

namespace ArmadilloAssault.GameState.Battle.Avatars
{
    public interface IAvatarListener : IWeaponListener
    {
        void AvatarHit(int hitIndex, int firedIndex, int damage);
        void AvatarKilled(int deadIndex, int? killIndex);
    }
}
