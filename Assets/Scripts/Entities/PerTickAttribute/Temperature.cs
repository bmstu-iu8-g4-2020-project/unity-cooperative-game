namespace Entities.PerTickAttribute
{
    public class Temperature : PerTickAttribute
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            ResistAttr = Player.PlayerController.LocalPlayer.Stats.TemperatureResist;
        }
    }
}
