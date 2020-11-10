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

    [CreateAssetMenu(fileName = "EquipmentItem", menuName = "Data/Items/EquipmentItem", order = 3)]
    public class EquipableItemData : UsableItemData
    {
        [field: SerializeField]
        public EquipmentSlot Slot { get; private set; }

        [SerializeField]
        private int attack;

        [SerializeField]
        private int defenseBonus;

        [SerializeField]
        private int maxHealthBonus;

        [SerializeField]
        [Range(0, 1)]
        private float temperatureResistBonus;

        private void ApplyBonus(PlayerStats playerStats)
        {
            playerStats.Defence.AddBaseBonus(defenseBonus);
            playerStats.MaxHealth.AddBaseBonus(maxHealthBonus);
            playerStats.TemperatureResist.AddAdditiveModifier(temperatureResistBonus);
        }

        public void CancelBonus(PlayerStats playerStats)
        {
            playerStats.Defence.AddBaseBonus(-defenseBonus);
            playerStats.MaxHealth.AddBaseBonus(-maxHealthBonus);
            playerStats.TemperatureResist.AddAdditiveModifier(-temperatureResistBonus);
        }

        public override bool CanUse(Entities.Player.Player player, ItemSlot slot) => player.Equipment.CanAdd(slot);

        public override void Use(Entities.Player.Player player, ItemSlot slot)
        {
            void OnComplete()
            {
                player.Equipment.SwapWithInventory(slot, player.Inventory);
                ApplyBonus(player.Stats);
            }

            DelayedOperationsManager.Instance.QueueOperation(new DelayedOperation(UsageTime, OnComplete));
            base.Use(player, slot);
        }
    }
}
