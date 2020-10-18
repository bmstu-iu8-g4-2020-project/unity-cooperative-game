using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(eventData.pointerClick.name);
            UIItemSlot uiItemSlot = eventData.pointerClick.GetComponent<UIItemSlot>();
            if (uiItemSlot != null)
            {
                if (!(UIContainerPanel.Instance.OpenedContainer is null))
                    UIContainerPanel.Instance.OpenedContainer.Remove(uiItemSlot.ItemSlot);
            }
        }
    }
}
