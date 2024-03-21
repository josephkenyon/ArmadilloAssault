using ArmadilloAssault.Assets;

namespace ArmadilloAssault.GameState.Battle.Items
{
    public interface IItemListener
    {
        int? TeamHeldIndex(Item item);
        bool BeingHeld(Item item);
    }
}
