using System;
using System.Collections.Generic;
using Mirror;
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
        Police,
        Inventory,
    }
    //TODO add lastOpeningTime and generate loot

    public class Container : NetworkBehaviour
    {
        [field: SerializeField]
        public string ContainerName { get; private set; } = "Container";

        [field: SerializeField]
        public ContainerType Type { get; private set; } = ContainerType.Kitchen;

        [SerializeField]
        private float maxWeight = 50;

        private float _currentWeight = 0;

        public delegate void SlotChangeDelegate(ItemSlot slot);

        public event SlotChangeDelegate OnSlotAdd;
        public event SlotChangeDelegate OnSlotChange;
        public event SlotChangeDelegate OnSlotRemove;


        public List<ItemSlot> Items { get; private set; } = new List<ItemSlot>();

        /// <summary>
        /// Try adding a 1 item to this container. If the container's maximum weight has already been reached, return false.
        /// </summary>
        /// <param name="slot">Slot with 1 item</param>
        public void AddOne(ItemSlot slot)
        {
            if (isClient)
            {
                CmdTryAdd(slot.GetData().title, slot.GetCondition());
            }
        }

        private bool TryAddOneLogic(ItemSlot slot)
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


        public void RemoveOne(ItemSlot slot)
        {
            if (isClient)
            {
                CmdTryRemove(slot.GetData().title, slot.GetCondition());
            }
        }

        private bool TryRemoveOneLogic(ItemSlot slot)
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

            AddOne(new ItemSlot(tempItems[index]));
        }

        public void TestRemoveRandomItem()
        {
            int index = Random.Range(0, Items.Count);
            if (0 <= index && index <= Items.Count)
            {
                RemoveOne(Items[index]);
            }
        }

        private static ItemData[] _allItems;
        private static Dictionary<ContainerType, ItemData[]> _lootForContainerType;

        private void Awake()
        {
            _allItems = Resources.LoadAll<ItemData>("Items");
            _lootForContainerType = new Dictionary<ContainerType, ItemData[]>
            {
                [ContainerType.Kitchen] = LoadAllOfType(ItemType.Food),
                [ContainerType.Default] = LoadAllOfType(ItemType.Basic),
                [ContainerType.Police] = LoadAllOfType(ItemType.Equipment),
                [ContainerType.Inventory] = null,
            };
        }

        private static ItemData[] LoadAllOfType(ItemType itemType)
        {
            List<ItemData> result = new List<ItemData>();
            foreach (var item in _allItems)
            {
                if (item.type == itemType)
                {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        private void Start()
        {
            GenerateLoot();
        }


        public void GenerateLoot()
        {
            if (!isServer) return;
            Items.Clear();
            ItemData[] tempItems = _lootForContainerType[Type];
            if (tempItems == null) return;


            for (int i = 0; i < 5 * TheData.Instance.GameData.generatedLootQuantity; i++)
            {
                int index = Random.Range(0, tempItems.Length);
                uint amount = (uint) Random.Range(1, 3);
                int condition = tempItems[index].isDegradable ? tempItems[index].maxCondition : -1;

                AddOne(new ItemSlot(tempItems[index], condition, amount));
            }
        }

        [ClientRpc]
        private void RpcAdd(string itemName, int condition)
        {
            if(isClient && isServer) return;
            TryAddOneLogic(new ItemSlot(ItemData.FindByName(itemName), condition));
        }

        [Command(ignoreAuthority = true)]
        private void CmdTryAdd(string itemName, int condition)
        {
            if (!TryAddOneLogic(new ItemSlot(ItemData.FindByName(itemName), condition))) return;
            RpcAdd(itemName, condition);
        }

        [ClientRpc]
        private void RpcRemove(string itemName, int condition)
        {
            if(isClient && isServer) return;
            TryRemoveOneLogic(new ItemSlot(ItemData.FindByName(itemName), condition));
        }

        [Command(ignoreAuthority = true)]
        private void CmdTryRemove(string itemName, int condition) //string itemName, int condition, int amount
        {
            if (!TryRemoveOneLogic(new ItemSlot(ItemData.FindByName(itemName), condition))) return;
            RpcRemove(itemName, condition);
        }
    }
}
