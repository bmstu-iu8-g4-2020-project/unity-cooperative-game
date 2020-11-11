using Data;
using Data.Items;
using Gameplay;

namespace Entities.Player
{
    public class Equipment : ItemContainer
    {
        public override bool CanAdd(ItemSlot slot) => slot.data is EquipableItemData;

        public override void RemoveOne(ItemSlot slot)
        {
            if (slot.data is EquipableItemData equipableItemData)
                equipableItemData.CancelBonus(PlayerController.LocalPlayer.Stats); //Cancel bonuses
            base.RemoveOne(slot);
        }

        public void SwapWithInventory(ItemSlot itemSlot, ItemContainer inventory)
        {
            var find = Items.Find(x =>
                x.data is EquipableItemData equipmentItemData && equipmentItemData.EquipmentSlot ==
                ((EquipableItemData) itemSlot.data).EquipmentSlot);
            if (find.hash != 0)
            {
                RemoveOne(find);
                inventory.AddOne(find);
            }

            inventory.RemoveOne(itemSlot);
            AddOne(itemSlot);
        }

        public bool TryGetItemInMainHand(out HandItem handItem)
        {
            handItem = Items.Find(x =>
                x.data is EquipableItemData equipmentItemData && equipmentItemData.EquipmentSlot ==
                EquipmentSlot.MainHand).data as HandItem;
            return handItem == null;
        }
    }
}
