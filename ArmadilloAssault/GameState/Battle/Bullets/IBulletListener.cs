using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Bullets
{
    public interface IBulletListener
    {
        void CreateEffect(Vector2 position, EffectType effectType, Direction? direction = null, double? weaponAngle = null);
        int GetTeamIndex(int playerIndex);
    }
}
