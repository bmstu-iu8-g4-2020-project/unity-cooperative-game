using Entities;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Data/Items/Armor", order = 4)]
    public class Armor : EquipableItemData
    {
        [SerializeField]
        private int defenseBonus;
        
        [SerializeField]
        private int maxHealthBonus;

        [SerializeField]
        [Range(0, 1)]
        private float temperatureResistBonus;

        protected override void ApplyBonus(PlayerStats playerStats)
        {
            playerStats.Defence.AddBaseBonus(defenseBonus);
            playerStats.MaxHealth.AddBaseBonus(maxHealthBonus);
            playerStats.TemperatureResist.AddAdditiveModifier(temperatureResistBonus);
        }

        public override void CancelBonus(PlayerStats playerStats)
        {
            playerStats.Defence.AddBaseBonus(-defenseBonus);
            playerStats.MaxHealth.AddBaseBonus(-maxHealthBonus);
            playerStats.TemperatureResist.AddAdditiveModifier(-temperatureResistBonus);
        }
    }
}
