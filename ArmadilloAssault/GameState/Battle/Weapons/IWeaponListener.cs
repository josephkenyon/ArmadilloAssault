using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public interface IWeaponListener
    {
        void CreateBullet(WeaponJson weaponConfiguration, Vector2 position, float angleTrajectory, float damageModifier, int playerIndex);
        void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null);
    }
}
