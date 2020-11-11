using Entities.Attributes;

namespace Entities.PerTickAttribute
{
    public class Thirst: PerTickAttribute
    {
        protected override void Start()
        {
            base.Start();
            ResistAttr = Player.PlayerController.LocalPlayer.Stats.ThirstResist;
        }
    }
}
