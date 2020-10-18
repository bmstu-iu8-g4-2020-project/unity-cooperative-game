using System;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public enum ContainerType
    {
        Default,
        Kitchen,
        Stationery,
        Police,
        Building
    }

    public class Container : Interactable //TODO do not inherit
    {
        [field: SerializeField]
        public string ContainerName { get; } = "Container";

        [field: SerializeField]
        public ContainerType Type { get; } = ContainerType.Kitchen;

        [SerializeField]
        private float maxWeight = 50; // todo or limit space not weight? 

        private float _currentWeight = 0;

        public delegate void SlotChangeDelegate(ItemSlot slot);

        public event SlotChangeDelegate OnSlotAdd;
        public event SlotChangeDelegate OnSlotChange;
        public event SlotChangeDelegate OnSlotRemove;


        public readonly List<ItemSlot> Items = new List<ItemSlot>();

        /// <summary>
        /// Try adding a 1 item to this container. If the container's maximum weight has already been reached, return false.
        /// </summary>
        public bool Add(ItemSlot slot)
        {
            if (!CanAdd(slot))
            {
                Debug.Log($"{ContainerName}: Weight limit reached");
                return false;
            }

            var index = Items.FindIndex(x => x.Equals(slot));
            if (index != -1)
            {
                bool hasAdded = Items[index].Increase();
                if (hasAdded)
                {
                    _currentWeight += slot.GetData().weight;
                    OnSlotChange?.Invoke(Items[index]);
                    return true;
                }
            }

            Items.Add(slot);
            _currentWeight += slot.GetData().weight;
            Debug.Log($"Add slot {slot.GetData().title} {slot.GetCondition()} in {GetType().Name}");
            OnSlotAdd?.Invoke(slot);
            return true;
        }


        public bool Remove(ItemSlot slot)
        {
            var index = Items.FindIndex(x => x.Equals(slot));
            if (index == -1) return false;
            if (slot.GetData().isStackable)
            {
                Items[index].Decrease();
                OnSlotChange?.Invoke(slot);
                if (slot.GetAmount() > 0) return true;
            }

            Items.Remove(slot);
            OnSlotRemove?.Invoke(slot);

            return true;
        }

        public bool CanAdd(ItemSlot slot)
        {
            return _currentWeight + slot.GetData().weight <= maxWeight;
        }

        public void TestAddRandomItem()
        {
            ItemData[] tempItems = new ItemData[3];
            tempItems[0] = Resources.Load<ItemData>("Items/Canned");
            tempItems[1] = Resources.Load<ItemData>("Items/BaseballBat");
            tempItems[2] = Resources.Load<ItemData>("Items/InsulatingTape");

            int index = Random.Range(0, 3);
            int condition = tempItems[index].isDegradable ? Random.Range(1, tempItems[index].maxCondition) : -1;

            Add(new ItemSlot(tempItems[index]));
        }

        public void TestRemoveRandomItem()
        {
            int index = Random.Range(0, Items.Count);
            if (index >= 0)
            {
                Remove(Items[index]);
            }
        }

        public void GenerateLoot()
        {
            #region Demonstration Code

            ItemData[] tempItems = new ItemData[3];
            tempItems[0] = Resources.Load<ItemData>("Items/Canned");
            tempItems[1] = Resources.Load<ItemData>("Items/BaseballBat");
            tempItems[2] = Resources.Load<ItemData>("Items/InsulatingTape");

            for (int i = 0; i < 2; i++)
            {
                int index = Random.Range(0, 3);
                uint amount = (uint) Random.Range(1, 100);
                int condition = tempItems[index].isDegradable ? Random.Range(1, tempItems[index].maxCondition) : -1;

                Items.Add(new ItemSlot(tempItems[index], condition, amount));
            }

            #endregion
        }

        // public override void OnInteract(PlayerCharacter character)
        // {
        //     base.OnInteract(character);
        //     UIContainerPanel.Instance.OpenContainer(this);
        // }
    }
}
