using ArmadilloAssault.Assets;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Avatars
{
    public interface IModeManagerListener
    {
        Dictionary<int, Avatar> GetAvatars();
    }
}
