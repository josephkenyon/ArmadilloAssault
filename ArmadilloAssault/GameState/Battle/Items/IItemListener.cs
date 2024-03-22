using ArmadilloAssault.Assets;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Items
{
    public interface IItemListener
    {
        bool BeingHeld(Item item);
        Dictionary<int, Avatar> GetAvatars();
    }
}
