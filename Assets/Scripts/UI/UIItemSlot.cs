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

        private bool _inInventory = false;//TODO impliment
        
        public void SetupSlot([NotNull] ItemSlot pItem)
        {
            icon.enabled = true;
            title.enabled = true;
            weightText.enabled = true;
            amountText.enabled = pItem.isStackable;
            condition.enabled = pItem.isDegradable;

            ItemSlot = pItem;
            icon.sprite = ItemSlot.icon;
            UpdateSlot();
        }

        public void UpdateSlot()
        {
            icon.sprite = ItemSlot.icon;
            title.text = ItemSlot.name;
            weightText.text = ItemSlot.TotalWeight.ToString(CultureInfo.InvariantCulture);

            if (ItemSlot.isStackable)
            {
                amountText.text = ItemSlot.GetAmount().ToString();
            }

            if (ItemSlot.isDegradable)
            {
                float conditionPercent = (float) ItemSlot.GetCondition() / ItemSlot.maxCondition;
                float barWidth = slotRect.rect.width * conditionPercent;
                condition.rectTransform.sizeDelta = new Vector2(barWidth, condition.rectTransform.sizeDelta.y);

                // Lerp color from green to red 
                condition.color = Color.Lerp(Color.red, Color.green, conditionPercent);
            }
        }
    }
}
