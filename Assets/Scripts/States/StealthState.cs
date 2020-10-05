using UnityEngine;

namespace States
{
    public class StealthState : MovementState
    {
        private bool _stealthHeld;

        public StealthState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Speed = Character.StealthSpeed;
        }

        public override void ProcessInput()
        {
            base.ProcessInput();
            _stealthHeld = Input.GetButton("Stealth");
        }

        public override void Update()
        {
            base.Update();
            Character.FaceToMouse();
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (!_stealthHeld)
            {
                StateMachine.ChangeState(Character.WalkState);
            }
        }
    }
}
