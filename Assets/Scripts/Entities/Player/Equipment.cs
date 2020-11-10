using Data;
using Gameplay;

namespace Entities.Player
{
    public class Equipment : ItemContainer
    {
        public override bool CanAdd(ItemSlot slot) => slot.data is EquipableItemData;

        public void SwapWithInventory(ItemSlot itemSlot, ItemContainer inventory)
        {
            var find = Items.Find(x =>
                x.data is EquipableItemData equipmentItemData && equipmentItemData.Slot ==
                ((EquipableItemData) itemSlot.data).Slot);
            if (find.hash != 0)
            {
                if (find.data is EquipableItemData equipableItemData)
                {
                    equipableItemData.CancelBonus(Player.LocalPlayer.Stats);
                }
                RemoveOne(find);
                inventory.AddOne(find);
            }

            inventory.RemoveOne(itemSlot);
            AddOne(itemSlot);
        }
    }
}
