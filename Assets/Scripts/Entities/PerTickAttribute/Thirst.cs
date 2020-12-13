namespace Entities.PerTickAttribute
{
    public class Thirst: PerTickAttribute
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            ResistAttr = Player.PlayerController.LocalPlayer.Stats.ThirstResist;
        }
    }
}
