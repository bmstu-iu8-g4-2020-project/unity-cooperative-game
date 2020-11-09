using Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private uint _amount;

        private void Start()
        {
            _amount = GetComponent<UIItemSlot>().ItemSlot.GetAmount();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_amount <= 0) return;
            Debug.Log(eventData.pointerClick.name);
            UIItemSlot uiItemSlot = /*eventData.pointerClick.*/ GetComponent<UIItemSlot>();
            if (!(uiItemSlot != null && eventData.button == PointerEventData.InputButton.Left)) return;
            if (UIController.Instance.ContainerUI.OpenedItemContainer is null
                || UIController.Instance.InventoryUI.OpenedItemContainer is null) return;

            UIContainerPanel from = uiItemSlot.GetComponentInParent<UIContainerPanel>();
            if (from == UIController.Instance.InventoryUI)
            {
                
            }
            
            var to = from == UIController.Instance.ContainerUI ? UIController.Instance.InventoryUI : UIController.Instance.ContainerUI;

            // if (!from.OpenedContainer.TryRemove(uiItemSlot.ItemSlot))
            //     return;
            
            //TODO fix 
            DelayedOperation.OperationDelegate onComplete = () =>
            {
                from.OpenedItemContainer.RemoveOne(uiItemSlot.ItemSlot);
                to.OpenedItemContainer.AddOne(uiItemSlot.ItemSlot.GetSlotWithOneItem());
            };

            _amount--;
            DelayedOperationsManager.Instance.QueueOperation(
                new DelayedOperation(uiItemSlot.ItemSlot.GetItemTransferTime(), onComplete));
        }
    }
}
