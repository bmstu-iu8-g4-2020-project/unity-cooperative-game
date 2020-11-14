using System;
using System.Collections.Generic;
using Entities.PerTickAttribute;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPerTickAttributes : MonoBehaviour
    {
        public Image thirst;
        public Image hunger;
        public Image temperature;
        public Image health;

        private Dictionary<string, Image> _attrBars;

        private Dictionary<string, Image> AttrBars => _attrBars ?? (_attrBars =
            new Dictionary<string, Image>
            {
                {nameof(Thirst), thirst},
                {nameof(Hunger), hunger},
                {nameof(Temperature), temperature},
                {nameof(Health), health}
            });

        public void UpdateBar(string attrName, float currentInPercent)
        {
            if (attrName != null && AttrBars.ContainsKey(attrName) && AttrBars[attrName] != null)
                AttrBars[attrName].fillAmount = (float) Math.Round(currentInPercent, 4);
        }
    }
}
