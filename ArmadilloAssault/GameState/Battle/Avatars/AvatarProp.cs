using ArmadilloAssault.Assets;
using ArmadilloAssault.GameState.Battle.Mode;

namespace ArmadilloAssault.GameState.Battle.Avatars
{
    public class AvatarProp(Avatar avatar, ModeType mode)
    {
        public bool Crowned { get; set; } = ModeType.Regicide == mode && avatar.Crowned;
    }
}
