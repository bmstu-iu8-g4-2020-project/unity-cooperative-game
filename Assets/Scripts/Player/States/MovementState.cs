using Data;
using Gameplay;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 
    /// </summary>
    public class MovementState : State
    {
        protected float Speed;

        private bool _playerStand; // todo create new state for standing
        // TODO create stealth-sprint state

        public MovementState(PlayerCharacter character, StateMachine stateMachine) : base(character, stateMachine)
        {
            Speed = TheData.Instance.PlayerData.WalkSpeed; //TODO depend on inventory weight 
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

            Character.Move(Speed, PlayerControls.Instance.GetMove().x, PlayerControls.Instance.GetMove().z);
        }
    }
}
