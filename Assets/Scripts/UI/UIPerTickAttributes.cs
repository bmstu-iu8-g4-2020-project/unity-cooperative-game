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

        private Dictionary<PerTickAttribute, Image> _attrBars;

        private Dictionary<PerTickAttribute, Image> AttrBars => _attrBars ?? (_attrBars =
            new Dictionary<PerTickAttribute, Image>
            {
                {Entities.Player.Player.LocalPlayer.Thirst, thirst},
                {Entities.Player.Player.LocalPlayer.Hunger, hunger},
                {Entities.Player.Player.LocalPlayer.Temperature, temperature},
                {Entities.Player.Player.LocalPlayer.Health, health}
            });

        public void UpdateBar(PerTickAttribute attr, float currentInPercent)
        {
            if (attr != null && AttrBars.ContainsKey(attr) && AttrBars[attr] != null)
                AttrBars[attr].fillAmount = (float) Math.Round(currentInPercent, 4);
        }
    }
}
