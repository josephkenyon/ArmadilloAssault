using ArmadilloAssault.Assets;

namespace ArmadilloAssault.GameState.Battle.Items
{
    public interface IItemListener
    {
        bool BeingHeld(Item item);
    }
}
