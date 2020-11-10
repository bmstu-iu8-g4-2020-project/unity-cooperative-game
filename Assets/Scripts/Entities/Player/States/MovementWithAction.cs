namespace Entities.Player.States
{
    public class MovementWithAction : MovementState
    {
        public MovementWithAction(Player character, StateMachine stateMachine) : base(character, stateMachine)
        {
            Speed = TheData.Instance.PlayerData.WalkSpeed * TheData.Instance.PlayerData.WalkWithActionSpeedMultiplier;
        }

        public override void Tick()
        {
            base.Tick();


            if (PlayerControls.Instance.IsHoldStealth())
                Speed = TheData.Instance.PlayerData.StealthSpeed *
                        TheData.Instance.PlayerData.WalkWithActionSpeedMultiplier;

            if (PlayerControls.Instance.IsHoldStealth()) Character.PlayerMovement.FaceToMouse();
        }
    }
}
