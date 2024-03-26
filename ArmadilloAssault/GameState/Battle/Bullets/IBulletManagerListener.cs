using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public interface IBulletManagerListener
    {
        void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null);
        int GetTeamIndex(int playerIndex);
        void BulletCreated(Bullet bullet);
        void BulletDeleted(int id);
    }
}
