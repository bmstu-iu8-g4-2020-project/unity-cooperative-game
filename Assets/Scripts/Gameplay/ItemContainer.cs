using Data;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class ItemContainer : NetworkBehaviour
    {
        public readonly SyncListItemSlot Items = new SyncListItemSlot();

        [SerializeField]
        private float maxWeight = 50;

        private float _currentWeight = 0;

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
                ItemSlot slotForChange = Items[index];
                bool hasIncreased = slotForChange.TryIncrease();
                if (hasIncreased)
                {
                    Items[index] = slotForChange;
                    _currentWeight += slot.GetWeightOf(1);
                    return;
                }
            }

            Items.Add(slot);
            _currentWeight += slot.GetWeightOf(1);
            Debug.Log($"Add slot {slot.name} {slot.GetCondition()} in {GetType().Name}");
        }


        [Command(ignoreAuthority = true)]
        public void RemoveOne(ItemSlot slot)
        {
            var index = GetItemIndexByNameAndCond(slot.name, slot._condition);
            if (index == -1) return;
            ItemSlot slotForChange = Items[index];
            if (slot.isStackable)
            {
                slotForChange.Decrease();
                Items[index] = slotForChange;
                if (slotForChange.GetAmount() > 0) return;
            }

            Items.RemoveAt(index);
        }

        public bool CanAdd(ItemSlot slot)
        {
            return _currentWeight + slot.GetWeightOf(1) <= maxWeight;
        }

        //Helper function to find an item in the slots
        public int GetItemIndexByNameAndCond(string itemName, int condition)
        {
            // (avoid FindIndex to minimize allocations)
            for (int i = 0; i < Items.Count; ++i)
            {
                ItemSlot slot = Items[i];
                if (slot.name == itemName && slot._condition == condition)
                    return i;
            }

            return -1;
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
