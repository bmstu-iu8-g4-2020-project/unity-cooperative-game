using System;
using System.Collections.Generic;
using Data;
using Data.Items;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// ItemSlot represents item in inventory or other storage with amount and condition.
    /// It only contains the dynamic item properties. Shouldn't exist if not store some item.
    /// </summary>
    [Serializable]
    public struct ItemSlot : IComparable
    {
        // hashcode used to reference the real ItemData
        // can't link to data directly because SyncList only supports simple types)
        [SerializeField]
        public int hash;

        [SerializeField]
        public uint _amount;

        [SerializeField]
        public int _condition; // -1 if not degradable

        /// Can't exist if not store some item. Item data Can't be null
        public ItemSlot([NotNull] ItemData data, int condition = 0, uint amount = 1)
        {
            if (data == null)
                throw new NullReferenceException($"{nameof(ItemData)} is null in ItemSlot constructor");

            hash = data.name.GetStableHashCode();
            _amount = data.isStackable ? amount : 1;
            condition = condition == 0 ? data.maxCondition : condition;
            _condition = data.isDegradable ? condition : -1;
        }

        ///Database item property access
        public ItemData data
        {
            get
            {
                if (!ItemData.dict.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ItemData with hash=" + hash + ".");
                return ItemData.dict[hash];
            }
        }

        /// Try to add item to this slot. If item is not stackable return false.
        public bool TryIncrease()
        {
            if (!data.isStackable) return false;
            _amount++;
            return true;
        }

        public bool Decrease()
        {
            _amount--;
            return true;
        }


        public void UseOnInventory(Entities.Player.PlayerController player)
        {
            if (!(data is UsableItemData usableItemData))
                return;
            usableItemData.UseOnInventory(player, this);
        }

        public bool CanUse(Entities.Player.PlayerController player) =>
            data is UsableItemData usableItemData && usableItemData.CanUse(player, this);

        #region Getters & Setters

        public string Name => data.name;
        public int MaxCondition => data.maxCondition;
        public bool IsStackable => data.isStackable;
        public bool IsDegradable => data.isDegradable;
        public Sprite Icon => data.icon;
        public float TotalWeight => data.weight * _amount;
        public uint GetAmount() => _amount;
        public int GetCondition() => _condition;
        public bool CheckCondition() => MaxCondition == -1 || _condition > 0;
        public float GetWeightOf(int amount) => data.weight * amount;

        public float ConditionPercent() =>
            _condition != -1 && MaxCondition != -1 ? (float) _condition / MaxCondition : 0;

        public ItemSlot GetSlotWithOneItem() => new ItemSlot(data, _condition);

        public float GetItemTransferTime() => TheData.Instance.PlayerData.ItemTransferRate * data.weight;

        #endregion

        #region Overrides For Collections

        public override bool Equals(object obj) => base.Equals(obj);

        public bool Equals(ItemSlot other) => hash == other.hash && _condition == other._condition;

        public override int GetHashCode() => (data.name + _condition).GetHashCode();

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return 1;
                case ItemSlot othSlot:
                    return String.Compare(data.name, othSlot.data.name, StringComparison.Ordinal) -
                           _condition.CompareTo(othSlot._condition);
                default:
                    throw new ArgumentException("Object is not a ItemSlot");
            }
        }

        #endregion
    }

    public class SyncListItemSlot : SyncList<ItemSlot>
    {
    }
}
