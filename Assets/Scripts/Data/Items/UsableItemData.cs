using System;
using Entities.Player;
using Gameplay;
using UnityEngine;

namespace Data
{
    public abstract class UsableItemData : ItemData
    {
        [field: SerializeField]
        protected float UsageTime { get; private set; } = 1;

        public virtual bool CanUse(Entities.Player.Player player, ItemSlot slot) => true;
        //Or use some interface for equipment and attribute resolver
        public virtual void UseOnInventory(Entities.Player.Player player, ItemSlot slot) => Debug.Log($"Use {name}");
    }
}
