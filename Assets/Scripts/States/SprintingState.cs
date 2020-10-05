using UnityEngine;

namespace States
{
    public class SprintingState : MovementState
    {
        private bool _sprintHeld;

        public SprintingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = Character.SprintSpeedMultiplier * Character.WalkSpeed;
        }

        public override void ProcessInput()
        {
            base.ProcessInput();
            _sprintHeld = Input.GetButton("Sprint"); // TODO can jam 
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (!_sprintHeld)
            {
                StateMachine.ChangeState(Character.WalkState);
            }
        }
    }
}
