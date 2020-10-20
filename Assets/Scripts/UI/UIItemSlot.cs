using System.Globalization;
using Gameplay;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This is slot representation for UI  
    /// </summary>
    public class UIItemSlot : MonoBehaviour
    {
        [SerializeField]
        private bool isCursor;

        [SerializeField]
        private RectTransform slotRect;

        [NotNull]
        public ItemSlot ItemSlot { get; set; }

        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI amountText;

        [SerializeField]
        private Image condition;

        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private TextMeshProUGUI weightText;

        private bool _inInventory = false;
        
        public void SetupSlot([NotNull] ItemSlot pItem)
        {
            icon.enabled = true;
            title.enabled = true;
            weightText.enabled = true;
            amountText.enabled = pItem.GetData().isStackable;
            condition.enabled = pItem.GetData().isDegradable;

            ItemSlot = pItem;
            icon.sprite = ItemSlot.GetData().icon;
            UpdateSlot();
        }

        public void UpdateSlot()
        {
            icon.sprite = ItemSlot.GetData().icon;
            title.text = ItemSlot.GetData().title;
            weightText.text = ItemSlot.TotalWeight.ToString(CultureInfo.InvariantCulture);

            if (ItemSlot.GetData().isStackable)
            {
                amountText.text = ItemSlot.GetAmount().ToString();
            }

            if (ItemSlot.GetData().isDegradable)
            {
                float conditionPercent = (float) ItemSlot.GetCondition() / ItemSlot.GetData().maxCondition;
                float barWidth = slotRect.rect.width * conditionPercent;
                condition.rectTransform.sizeDelta = new Vector2(barWidth, condition.rectTransform.sizeDelta.y);

                // Lerp color from green to red 
                condition.color = Color.Lerp(Color.red, Color.green, conditionPercent);
            }
        }
    }
}
