using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAttribute : MonoBehaviour
    {
        public Image hunger;
        public Image thirst;

        private Dictionary<AttributeType, Image> _attrBar;

        private void Start()
        {
            _attrBar = new Dictionary<AttributeType, Image>
            {
                {AttributeType.Hunger, hunger},
                {AttributeType.Thirst, thirst},
            };
        }

        public void UpdateBar(AttributeType attrType, float currentInPercent)
        {
            _attrBar[attrType].fillAmount = currentInPercent / 100;
        }
    }
}
