using UnityEngine;

namespace States
{
    public class WalkState : MovementState
    {
        private bool _sprint;
        private bool _stealth;

        public WalkState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _stealth = false;
            _sprint = false;
        }

        public override void ProcessInput()
        {
            base.ProcessInput();
            _stealth = Input.GetButtonDown("Stealth");
            _sprint = Input.GetButtonDown("Sprint");
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (_stealth)
            {
                StateMachine.ChangeState(Character.StealthState);
            }
            else if (_sprint)
            {
                StateMachine.ChangeState(Character.SprintState);
            }
        }
    }
}
