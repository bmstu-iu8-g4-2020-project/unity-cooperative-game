namespace Entities.Player.States
{
    /// <summary>
    ///     Slow movement and rotation control
    /// </summary>
    public class StealthState : MovementState
    {
        public StealthState(PlayerController character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = TheData.Instance.PlayerData.StealthSpeed;
            AttractionSource.Radius = AttractionSource.RadiusForStealth;
        }

        public override void Tick()
        {
            base.Tick();
            Character.PlayerMovement.FaceToMouse();
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (!PlayerControls.Instance.IsHoldStealth()) StateMachine.ChangeState(Character.WalkState);
        }
    }
}
