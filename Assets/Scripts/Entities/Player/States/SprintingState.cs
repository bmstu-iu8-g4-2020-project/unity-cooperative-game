namespace Entities.Player.States
{
    public class SprintingState : MovementState
    {
        public SprintingState(PlayerController character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = TheData.Instance.PlayerData.SprintSpeedMultiplier * TheData.Instance.PlayerData.WalkSpeed;
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (!PlayerControls.Instance.IsHoldSprint()) StateMachine.ChangeState(Character.WalkState);
        }
    }
}
