using System.Globalization;
using Gameplay;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     This is slot representation for UI
    /// </summary>
    public class UIItemSlot : MonoBehaviour
    {
        [SerializeField]
        private bool isCursor;

        [SerializeField]
        private RectTransform slotRect;

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

        private bool _inInventory = false; //TODO impliment

        [NotNull]
        public ItemSlot ItemSlot { get; set; }

        public void SetupSlot([NotNull] ItemSlot pItem)
        {
            icon.enabled = true;
            title.enabled = true;
            weightText.enabled = true;
            amountText.enabled = pItem.IsStackable;
            condition.enabled = pItem.IsDegradable;

            ItemSlot = pItem;
            icon.sprite = ItemSlot.Icon;
            UpdateSlot();
        }

        public void UpdateSlot()
        {
            icon.sprite = ItemSlot.Icon;
            title.text = ItemSlot.Name;
            weightText.text = ItemSlot.TotalWeight.ToString(CultureInfo.InvariantCulture);

            if (ItemSlot.IsStackable) amountText.text = ItemSlot.GetAmount().ToString();

            if (!ItemSlot.IsDegradable) return;
            var conditionPercent = (float) ItemSlot.GetCondition() / ItemSlot.MaxCondition;
            var barWidth = slotRect.rect.width * conditionPercent;
            condition.rectTransform.sizeDelta = new Vector2(barWidth, condition.rectTransform.sizeDelta.y);

            // Lerp color from green to red 
            condition.color = Color.Lerp(Color.red, Color.green, conditionPercent);
        }
    }
}
