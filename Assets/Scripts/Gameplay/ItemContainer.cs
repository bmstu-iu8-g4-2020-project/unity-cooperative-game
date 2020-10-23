using System;
using Data;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class ItemContainer : NetworkBehaviour
    {
        public SyncListItemSlot Items = new SyncListItemSlot();

        [SerializeField]
        private float maxWeight = 50;
        
        private float _currentWeight = 0;

        public delegate void SlotChangeDelegate(ItemSlot slot);
        public delegate void ContainerUpdateDelegate(SyncListItemSlot.Operation op, int index, ItemSlot oldItem, ItemSlot newItem);

        public event SlotChangeDelegate OnSlotAdd;
        public event SlotChangeDelegate OnSlotChange;
        public event SlotChangeDelegate OnSlotRemove;

        /// Try adding a 1 item to this container. If the container's maximum weight has already been reached, return false.
        [Command(ignoreAuthority = true)]
        public void AddOne(ItemSlot slot)
        {
            if (!CanAdd(slot))
            {
                Debug.Log($"{gameObject.name} Container: Weight limit reached");
                return;
            }

            var index = GetItemIndexByNameAndCond(slot.name, slot._condition);
            if (index != -1)
            {
                ItemSlot slotForChange  = Items[index];
                bool hasIncreased = slotForChange.TryIncrease();
                if (hasIncreased)
                {
                    //TODO use command to update item in list
                    Items[index] = slotForChange;
                    _currentWeight += slot.GetWeightOf(1);
                    // RpcOnSlotChange(slotForChange);
                    return;
                }
            }

            Items.Add(slot);
            _currentWeight += slot.GetWeightOf(1);
            Debug.Log($"Add slot {slot.name} {slot.GetCondition()} in {GetType().Name}");
            // RpcOnSlotAdd(slot);
        }


        [Command(ignoreAuthority = true)]
        public void RemoveOne(ItemSlot slot)
        {
            //TODO if amount == 1 remove

            var index = GetItemIndexByNameAndCond(slot.name, slot._condition);
            if (index == -1) return;
            ItemSlot slotForChange  = Items[index];
            if (slot.isStackable)
            {
                slotForChange.Decrease();
                //TODO use command to update item in list
                Items[index] = slotForChange;
                // RpcOnSlotChange(slotForChange);
                if (slotForChange.GetAmount() > 0) return;
                //TODO add all players that interact with object to update their ui
            }

            Items.RemoveAt(index);
            // RpcOnSlotRemove(slot);
        }
        
        public bool CanAdd(ItemSlot slot)
        {
            return _currentWeight + slot.GetWeightOf(1) <= maxWeight;
        }

        // helper function to find an item in the slots
        public int GetItemIndexByNameAndCond(string itemName, int condition)
        {
            // (avoid FindIndex to minimize allocations)
            for (int i = 0; i < Items.Count; ++i)
            {
                ItemSlot slot = Items[i];
                if (slot._condition == condition && slot.name == itemName)
                    return i;
            }

            return -1;
        }

        #region Unity callbacks

        private void Start()
        {
            // Items.Callback += OnContainerUpdate;
        }
        
        //Должен вызываться на клиентах при изменении SyncList
        private void OnContainerUpdate(SyncList<ItemSlot>.Operation op, int itemindex, ItemSlot olditem, ItemSlot newitem)
        {
            switch (op)
            {
                case SyncList<ItemSlot>.Operation.OP_ADD:
                    OnSlotAdd?.Invoke(newitem);
                    break;
                case SyncList<ItemSlot>.Operation.OP_CLEAR:
                    break;
                case SyncList<ItemSlot>.Operation.OP_INSERT:
                    OnSlotAdd?.Invoke(newitem);
                    break;
                case SyncList<ItemSlot>.Operation.OP_REMOVEAT:
                    OnSlotRemove?.Invoke(olditem);
                    break;
                case SyncList<ItemSlot>.Operation.OP_SET:
                    OnSlotChange?.Invoke(newitem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        #endregion

        [ClientRpc]
        void RpcOnSlotAdd(ItemSlot slot)
        {
            OnSlotAdd?.Invoke(slot);
        }

        [ClientRpc]
        void RpcOnSlotRemove(ItemSlot slot)
        {
            OnSlotRemove?.Invoke(slot);
        }

        [ClientRpc]
        void RpcOnSlotChange(ItemSlot slot)
        {
            OnSlotChange?.Invoke(slot);
        }

       
        
        #region Test

        [Command(ignoreAuthority = true)]
        public void TestAddRandomItem()
        {
            ItemData[] tempItems = new ItemData[3];
            tempItems[0] = Resources.Load<ItemData>("Items/Canned");
            tempItems[1] = Resources.Load<ItemData>("Items/BaseballBat");
            tempItems[2] = Resources.Load<ItemData>("Items/InsulatingTape");

            int index = Random.Range(0, 3);
            int condition = tempItems[index].isDegradable ? Random.Range(1, tempItems[index].maxCondition) : -1;

            AddOne(new ItemSlot(tempItems[index]));
        }

        [Command(ignoreAuthority = true)]
        public void TestRemoveRandomItem()
        {
            int index = Random.Range(0, Items.Count);
            if (0 <= index && index <= Items.Count)
            {
                RemoveOne(Items[index]);
            }
        }

        #endregion
    }
}
