using System;
using System.Collections.Generic;
using System.Text;
using Gameplay;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace UI
{
    class PrefabNotFoundException : Exception
    {
        public PrefabNotFoundException(string s) : base(s)
        {
        }
    }

    /// <summary>
    /// Control UI panel for container. Show, hide, update while showing 
    /// </summary>
    public class UIContainerPanel : MonoBehaviour
    {
        [Header("Container UI Elements")]
        public GameObject hidablePanel; //Panel that changes visibility

        public TextMeshProUGUI title;
        public Transform contentWindow; // The GridLayoutWindow that using to display our UIItemSlots.

        GameObject _uiSlotPrefab; // The prefab of the UIItemSlots.

        public bool IsOpen { get; private set; }


        private void Start()
        {
            var pathToPrefab = "Prefabs/UIItemSlotList";
            _uiSlotPrefab = Resources.Load<GameObject>(pathToPrefab);
            if (_uiSlotPrefab == null)
            {
                throw new PrefabNotFoundException($"{pathToPrefab} not found in script attached to {gameObject.name}");
            }

            hidablePanel.SetActive(false);
            IsOpen = false;
        }


        private List<KeyValuePair<ItemSlot, UIItemSlot>> _slots;

        [CanBeNull]
        public Gameplay.ItemContainer OpenedItemContainer { get; private set; }

        /// Open container UI panel. 
        public void OpenContainer(Gameplay.ItemContainer itemContainer)
        {
            if (IsOpen) CloseContainer();
            IsOpen = true;
            _slots = new List<KeyValuePair<ItemSlot, UIItemSlot>>();
            //TODO sort
            //Sorting items in container
            // if (_items.Count > 0)
            // {
            //     _items.Sort();
            // }

            hidablePanel.SetActive(true);

            //Subscribe on SyncList update
            itemContainer.Items.Callback += OnContainerUpdated;

            // Loop through each item in the given items list and instantiate a new UIItemSlot prefab for each one.
            for (int i = 0; i < itemContainer.Items.Count; i++)
            {
                CreateUISlot(i, itemContainer.Items[i]);
            }

            OpenedItemContainer = itemContainer;
        }


        private void UpdateSlot(ItemSlot oldSlot, ItemSlot newSlot)
        {
            var index = _slots.FindIndex(x => x.Key.Equals(oldSlot));
            _slots[index] = new KeyValuePair<ItemSlot, UIItemSlot>(newSlot, _slots[index].Value);
            _slots[index].Value.ItemSlot = newSlot;
            //TODO replace ItenSlot var in UIItemSlot and in _slots
            _slots[index].Value.UpdateSlot();
        }

        private void OnSlotRemove(ItemSlot slot)
        {
            var pair = _slots.Find(x => x.Key.Equals(slot));
            var index = _slots.IndexOf(pair);
            _slots.Remove(pair);
            Destroy(pair.Value.gameObject);
            for (int i = index; i < _slots.Count; i++)
            {
                StringBuilder sb = new StringBuilder(_slots[i].Value.gameObject.name) {[0] = i.ToString()[0]};
                _slots[i].Value.gameObject.name = sb.ToString();
            }
        }

        private void OnSlotAdd(ItemSlot slot)
        {
            bool hasAdded = false;
            for (int i = 0; i < _slots.Count; i++)
            {
                var compareTo = slot.CompareTo(_slots[i].Key);
                if (!hasAdded && compareTo <= 0)
                {
                    var uiSlotObject = Instantiate(_uiSlotPrefab, contentWindow);
                    uiSlotObject.transform.SetSiblingIndex(i);
                    uiSlotObject.name = i + " " + slot.name;

                    var uiSlot = uiSlotObject.GetComponent<UIItemSlot>();
                    uiSlot.SetupSlot(slot);
                    _slots.Insert(i, new KeyValuePair<ItemSlot, UIItemSlot>(slot, uiSlot));
                    hasAdded = true;
                }
                else
                {
                    //todo mb dont change names here or everywhere
                    StringBuilder sb = new StringBuilder(_slots[i].Value.gameObject.name) {[0] = i.ToString()[0]};
                    _slots[i].Value.gameObject.name = sb.ToString();
                }
            }

            //If container empty
            if (!hasAdded)
            {
                CreateUISlot(0, slot);
            }
        }

        void OnContainerUpdated(SyncListItemSlot.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
        {
            switch (op)
            {
                case SyncListItemSlot.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    OnSlotAdd(newItem);
                    break;
                case SyncListItemSlot.Operation.OP_CLEAR:
                    // list got cleared
                    break;
                case SyncListItemSlot.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item
                    OnSlotAdd(newItem);
                    break;
                case SyncListItemSlot.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed
                    OnSlotRemove(oldItem);
                    break;
                case SyncListItemSlot.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    UpdateSlot(oldItem, newItem);
                    break;
            }
        }

        void CreateUISlot(int namePrefix, ItemSlot slot)
        {
            //Make sure our GridLayoutWindow is set as the parent of the new UIItemSlot object.
            GameObject uiSlotObject = Instantiate(_uiSlotPrefab, contentWindow);
            uiSlotObject.name = namePrefix + " " + slot.name;

            var uiSlot = uiSlotObject.GetComponent<UIItemSlot>();
            uiSlot.SetupSlot(slot); //Setup icon\text

            //Add the new slot to our UISlots list so we can find it later.
            _slots.Add(new KeyValuePair<ItemSlot, UIItemSlot>(slot, uiSlot));
        }


        public void CloseContainer()
        {
            // Loop through each slot, detatch it from its ItemSlot and delete the GameObject.
            foreach (var slot in _slots)
            {
                Destroy(slot.Value.gameObject);
            }

            if (!(OpenedItemContainer is null))
            {
                OpenedItemContainer.Items.Callback -= OnContainerUpdated;
            }

            OpenedItemContainer = null;
            // Clear the list and deactivate the container window.
            _slots.Clear();
            hidablePanel.SetActive(false);
            IsOpen = false;
        }
    }
}
