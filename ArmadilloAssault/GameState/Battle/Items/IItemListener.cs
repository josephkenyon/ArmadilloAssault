using ArmadilloAssault.Assets;

namespace ArmadilloAssault.GameState.Battle.Items
{
    public interface IItemListener
    {
        bool CanBePickedUp(Item item);
    }
}
