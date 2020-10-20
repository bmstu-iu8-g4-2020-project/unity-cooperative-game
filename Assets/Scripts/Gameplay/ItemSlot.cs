using System;
using Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// ItemSlot represents item in inventory or other storage with amount and condition.
    /// Shouldn't exist if not store some item.
    /// </summary>
    public class ItemSlot : IComparable
    {
        [NotNull]
        private readonly ItemData _data;

        private uint _amount;

        //TODO If condition decrease- create new slot or find with same cond.
        private readonly int _condition; // -1 if not degradable

        /// <summary>
        /// Can't exist if not store some item.
        /// </summary>
        /// <param name="data">Can't be null.</param>
        /// <param name="condition">Condition of items in slot. Can't be 0.</param>
        /// <param name="amount">If Item is not stackable it set to 1</param>
        /// <exception cref="NullReferenceException">ItemData data</exception>
        public ItemSlot([NotNull] ItemData data, int condition = 0, uint amount = 1)
        {
            if (data == null)
            {
                throw new NullReferenceException($"{nameof(ItemData)} is null in {GetType().Name} constructor");
            }

            _data = data;
            _amount = _data.isStackable ? amount : 1;
            condition = condition == 0 ? data.maxCondition : condition;
            _condition = _data.isDegradable ? condition : -1;
        }

        /// <summary>
        /// Try to add item to this slot. If item is not stackable return false.
        /// </summary>
        public bool Increase()
        {
            if (!_data.isStackable) return false;
            _amount++;
            return true;
        }

        public bool Decrease()
        {
            _amount--;
            return true;
        }

        public ItemSlot GetSlotWithOneItem()
        {
            return new ItemSlot(_data, _condition);
        }

        // public static ItemSlot GetSlotWithOneItem(ItemSlot slot)
        // {
        //     return new ItemSlot(slot._data, slot._condition);
        // }

        #region Getters & Setters

        public ItemData GetData() => _data;
        public uint GetAmount() => _amount;
        public int GetCondition() => _condition;
        public float TotalWeight => _data.weight * _amount;
        public float GetWeightOf(int amount) => _data.weight * amount;

        #endregion

        public float GetItemTransferTime()
        {
            return TheData.Instance.PlayerData.ItemTransferRate * _data.weight;
        }

        private static bool Compare(ItemSlot slot1, ItemSlot slot2)
        {
            // Items can store in one slot only if their condition equals
            if (slot1._data != slot2._data || slot1._condition != slot2._condition)
                return false;

            return true;
        }

        public void Use()
        {
            
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemSlot);
        }

        protected bool Equals(ItemSlot other)
        {
            return _data.title == other._data.title && _condition == other._condition;
        }

        public override int GetHashCode()
        {
            return (_data.title + _condition).GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is ItemSlot othSlot)
                return String.Compare(this._data.title, othSlot._data.title, StringComparison.Ordinal) -
                       _condition.CompareTo(othSlot._condition);

            throw new ArgumentException("Object is not a ItemSlot");
        }
    }
}
