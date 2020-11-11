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
        public EquipmentSlot EquipmentSlot { get; protected set; }

        protected virtual void ApplyBonus(PlayerStats playerStats)
        {
        }

        public virtual void CancelBonus(PlayerStats playerStats)
        {
        }

        public override bool CanUse(PlayerController player, ItemSlot slot) => true;

        public override void UseOnInventory(PlayerController player, ItemSlot slot)
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
