namespace Entities.Player.States
{
    /// <summary>
    /// </summary>
    public class MovementState : State
    {
        private bool _playerStand; // todo create new state for standing

        protected float Speed;
        // TODO create stealth-sprint state

        public MovementState(Player character, StateMachine stateMachine) : base(character, stateMachine)
        {
            Speed = TheData.Instance.PlayerData.WalkSpeed; //TODO depending on inventory weight 
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = TheData.Instance.PlayerData.WalkSpeed;
            _playerStand = false;
        }

        public override void Tick()
        {
            base.Tick();

            Character.PlayerMovement.Move(Speed, PlayerControls.Instance.GetMove().x,
                PlayerControls.Instance.GetMove().z);
        }
    }
}
