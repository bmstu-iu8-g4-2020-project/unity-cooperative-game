namespace Entities.PerTickAttribute
{
    public class Hunger : PerTickAttribute
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            ResistAttr = Player.PlayerController.LocalPlayer.Stats.HungerResist;
        }
    }
}