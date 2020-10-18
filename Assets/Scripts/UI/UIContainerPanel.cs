using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gameplay;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private bool isOpen;

        #region singltone

        public static UIContainerPanel Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Removed duplicate singltone script on {gameObject.name}");
                Destroy(this);
            }
        }

        #endregion


        private void Start()
        {
            var pathToPrefab = "Prefabs/UIItemSlotList";
            _uiSlotPrefab = Resources.Load<GameObject>(pathToPrefab);
            if (_uiSlotPrefab == null)
            {
                throw new PrefabNotFoundException($"{pathToPrefab} not found in script attached to {gameObject.name}");
            }

            hidablePanel.SetActive(false);
            isOpen = false;
        }


        private List<KeyValuePair<ItemSlot, UIItemSlot>> _slots;

        [CanBeNull]
        public Container OpenedContainer { get; private set; }

        /// <summary>
        /// Snow container UI panel.
        /// </summary>
        public void OpenContainer(Container container)
        {
            if (isOpen) return;
            isOpen = true;
            _slots = new List<KeyValuePair<ItemSlot, UIItemSlot>>();
            //Sorting items in container
            container.Items.Sort();

            hidablePanel.SetActive(true);

            title.text = container.ContainerName.ToUpper(); // Set the name of the container.

            container.OnSlotChange += UpdateSlot;
            container.OnSlotAdd += OnSlotAdd;
            container.OnSlotRemove += OnSlotRemove;
            _slots.Clear();

            var i = 0;
            // Loop through each item in the given items list and instantiate a new UIItemSlot prefab for each one.
            foreach (var slot in container.Items)
            {
                CreateUISlot(i, slot);
                i++;
            }

            OpenedContainer = container;
        }


        private void UpdateSlot(ItemSlot slot)
        {
            var pair = _slots.Find(x => x.Key.Equals(slot));
            pair.Value.UpdateSlot();
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
                    uiSlotObject.name = i + " " + slot.GetData().name;

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

        void CreateUISlot(int namePrefix, ItemSlot slot)
        {
            //Make sure our GridLayoutWindow is set as the parent of the new UIItemSlot object.
            GameObject uiSlotObject = Instantiate(_uiSlotPrefab, contentWindow);
            uiSlotObject.name = namePrefix + " " + slot.GetData().name;

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

            if (!(OpenedContainer is null))
            {
                OpenedContainer.OnSlotChange -= UpdateSlot;
                OpenedContainer.OnSlotAdd -= OnSlotAdd;
            }

            OpenedContainer = null;
            // Clear the list and deactivate the container window.
            _slots.Clear();
            hidablePanel.SetActive(false);
            isOpen = false;
        }
    }
}
