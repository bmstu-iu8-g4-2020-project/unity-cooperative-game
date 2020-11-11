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
                {Entities.Player.PlayerController.LocalPlayer.Thirst, thirst},
                {Entities.Player.PlayerController.LocalPlayer.Hunger, hunger},
                {Entities.Player.PlayerController.LocalPlayer.Temperature, temperature},
                {Entities.Player.PlayerController.LocalPlayer.Health, health}
            });

        public void UpdateBar(PerTickAttribute attr, float currentInPercent)
        {
            if (attr != null && AttrBars.ContainsKey(attr) && AttrBars[attr] != null)
                AttrBars[attr].fillAmount = (float) Math.Round(currentInPercent, 4);
        }
    }
}
