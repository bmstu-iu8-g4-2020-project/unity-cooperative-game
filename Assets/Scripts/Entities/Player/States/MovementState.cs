using Gameplay;

namespace Entities.Player.States
{
    /// <summary>
    /// </summary>
    public class MovementState : State
    {
        private bool _playerStand; // todo create new state for standing

        protected float Speed;

        protected PlayerSoundAttractionSource AttractionSource;
        // TODO create stealth-sprint state

        public MovementState(PlayerController character, StateMachine stateMachine) : base(character, stateMachine)
        {
            Speed = TheData.Instance.PlayerData.WalkSpeed; //TODO depending on inventory weight
            AttractionSource = character.AttractionSource;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = TheData.Instance.PlayerData.WalkSpeed;
            _playerStand = false;
            AttractionSource.Radius = 0;
        }

        public override void Tick()
        {
            base.Tick();

            Character.PlayerMovement.Move(Speed, PlayerControls.Instance.GetMove().x,
                PlayerControls.Instance.GetMove().z);
        }
    }
}
