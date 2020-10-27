using System;
using System.Collections.Generic;
using Data;
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


        public void Use()
        {
        }

        #region Getters & Setters

        public string name => data.name;
        public int maxCondition => data.maxCondition;
        public bool isStackable => data.isStackable;
        public bool isDegradable => data.isDegradable;
        public Sprite icon => data.icon;
        public float TotalWeight => data.weight * _amount;
        public uint GetAmount() => _amount;
        public int GetCondition() => _condition;
        public bool CheckCondition() => maxCondition == -1 || _condition > 0;
        public float GetWeightOf(int amount) => data.weight * amount;

        public float ConditionPercent() =>
            _condition != -1 && maxCondition != -1 ? (float) _condition / maxCondition : 0;

        public ItemSlot GetSlotWithOneItem()
        {
            return new ItemSlot(data, _condition);
        }

        public float GetItemTransferTime()
        {
            return TheData.Instance.PlayerData.ItemTransferRate * data.weight;
        }

        #endregion

        #region Overrides For Collections

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(ItemSlot other)
        {
            return hash == other.hash && _condition == other._condition;
        }

        public override int GetHashCode()
        {
            return (data.name + _condition).GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is ItemSlot othSlot)
                return String.Compare(this.data.name, othSlot.data.name, StringComparison.Ordinal) -
                       _condition.CompareTo(othSlot._condition);

            throw new ArgumentException("Object is not a ItemSlot");
        }

        #endregion
    }

    public class SyncListItemSlot : SyncList<ItemSlot>
    {
    }
}
