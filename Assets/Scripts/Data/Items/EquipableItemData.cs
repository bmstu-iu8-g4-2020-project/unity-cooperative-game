using Entities;
using Entities.Player;
using Gameplay;
using UnityEngine;

namespace Data
{
    public enum EquipmentSlot : byte
    {
        MainHand,
        AdditionalHand,
        Head,
        Body,
        Legs,
        Arms
    }

    public abstract class EquipableItemData : UsableItemData
    {
        [field: SerializeField]
        public EquipmentSlot EquipmentSlot { get; private set; }

        protected virtual void ApplyBonus(PlayerStats playerStats)
        {
        }

        public virtual void CancelBonus(PlayerStats playerStats)
        {
        }

        public override bool CanUse(Player player, ItemSlot slot) => true;

        public override void UseOnInventory(Player player, ItemSlot slot)
        {
            void OnComplete()
            {
                player.Equipment.SwapWithInventory(slot, player.Inventory);
                ApplyBonus(player.Stats);
            }

            DelayedOperationsManager.Instance.QueueOperation(new DelayedOperation(UsageTime, OnComplete));
            base.UseOnInventory(player, slot);
        }
    }
}
