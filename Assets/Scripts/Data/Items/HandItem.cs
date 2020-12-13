using Entities;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "HandItem", menuName = "Data/Items/HandItem", order = 3)]
    public class HandItem : EquipableItemData
    {
        [SerializeField]
        private int attack;

        [field: SerializeField]
        //The number of targets that can be attacked at a time
        public uint TargetsPerHit { get; private set; } = 1;

        [field: SerializeField]
        public float CooldownTime { get; private set; } = 1;
        protected override void ApplyBonus(PlayerStats playerStats) => playerStats.Attack.AddBaseBonus(attack);
        public override void CancelBonus(PlayerStats playerStats) => playerStats.Attack.AddBaseBonus(-attack);
    }
}
