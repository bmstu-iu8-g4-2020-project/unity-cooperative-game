using Entities;
using Entities.Player;
using Gameplay;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "FoodItemData", menuName = "Data/Items/FoodItemData", order = 4)]
    public class FoodItemData : UsableItemData
    {
        public int usageHealth;
        public int usageWarming;
        public int usageNutrition;
        public int usageHydration;

        private void ApplyEffects(PlayerController player)
        {
            player.Temperature.Current += usageWarming;
            player.Hunger.Current += usageNutrition;
            player.Thirst.Current += usageHydration;
            player.Health.Current += usageHealth;
        }

        public override void UseOnInventory(PlayerController player, ItemSlot slot)
        {
            void OnComplete()
            {
                player.Inventory.RemoveOne(slot);
                ApplyEffects(player.GetComponent<PlayerController>());
            }

            DelayedOperationsManager.Instance.QueueOperation(new DelayedOperation(UsageTime, OnComplete));
            base.UseOnInventory(player, slot);
        }
    }
}
