using Entities.Attributes;

namespace Entities.PerTickAttribute
{
    public class Temperature : PerTickAttribute
    {
        protected override void Start()
        {
            base.Start();
            ResistAttr = Player.Player.LocalPlayer.Stats.TemperatureResist;
        }
    }
}
