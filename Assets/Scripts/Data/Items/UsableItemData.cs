using Entities.Player;
using Gameplay;
using UnityEngine;

namespace Data
{
    public abstract class UsableItemData : ItemData
    {
        private EquipmentSlot EquipmentSlot  = EquipmentSlot.MainHand;
        [field: SerializeField]
        protected float UsageTime { get; private set; } = 1;

        public virtual bool CanUse(PlayerController player, ItemSlot slot) => true;
        //Or use some interface for equipment and attribute resolver
        public virtual void UseOnInventory(PlayerController player, ItemSlot slot) => Debug.Log($"Use {name}");
    }
}
