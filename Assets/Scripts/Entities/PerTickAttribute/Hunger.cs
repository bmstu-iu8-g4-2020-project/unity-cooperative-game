using System;
using Entities.Attributes;

namespace Entities.PerTickAttribute
{
    public class Hunger: PerTickAttribute
    {
        protected override void Start()
        {
            base.Start();
            ResistAttr = Player.PlayerController.LocalPlayer.Stats.HungerResist;
        }
    }
}
