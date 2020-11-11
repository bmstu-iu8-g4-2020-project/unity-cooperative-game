using System;
using Entities;
using Entities.Player;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private uint _clickCounter;

        public void OnPointerClick(PointerEventData eventData)
        {
            var uiItemSlot = GetComponent<UIItemSlot>();
            if (uiItemSlot == null)
                return;

            //todo потенциальная ошибка: ниже предполагается, что слот может быть ребенком только двух UI game obejct'ов
            var from = uiItemSlot.GetComponentInParent<UIContainerPanel>().OpenedItemContainer;
            if (from == null) throw new NullReferenceException("UI item slot haven't parent with opened container");

            //TODO use polymorphism 
            switch (eventData.button)
            {
                //Use Item
                case PointerEventData.InputButton.Right
                    when from == UIController.Instance.InventoryUI.OpenedItemContainer:
                {
                    if (uiItemSlot.ItemSlot.CanUse(PlayerController.LocalPlayer))
                        uiItemSlot.ItemSlot.UseOnInventory(PlayerController.LocalPlayer);

                    break;
                }
                //Move Item
                case PointerEventData.InputButton.Left when _clickCounter <= uiItemSlot.ItemSlot.GetAmount():
                {
                    _clickCounter++;
                    var to = from == UIController.Instance.ContainerUI.OpenedItemContainer
                        ? UIController.Instance.InventoryUI.OpenedItemContainer
                        : UIController.Instance.ContainerUI.OpenedItemContainer;

                    MoveItem(uiItemSlot, @from, to);
                    break;
                }
            }
        }

        void MoveItem(UIItemSlot uiItemSlot, ItemContainer from, ItemContainer to)
        {
            if (UIController.Instance.ContainerUI.OpenedItemContainer == null
                || UIController.Instance.InventoryUI.OpenedItemContainer == null)
                return;

            //TODO block items for another players
            void OnComplete()
            {
                from.RemoveOne(uiItemSlot.ItemSlot);
                to.AddOne(uiItemSlot.ItemSlot.GetSlotWithOneItem());
            }

            var delayedOperation = new DelayedOperation(uiItemSlot.ItemSlot.GetItemTransferTime(), OnComplete);
            DelayedOperationsManager.Instance.QueueOperation(delayedOperation);
        }
    }
}
