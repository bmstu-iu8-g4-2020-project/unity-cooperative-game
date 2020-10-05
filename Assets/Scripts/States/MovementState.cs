using UnityEngine;

namespace States
{
    public class MovementState : State
    {
        protected float Speed;

        private float _horizontalInput;
        private float _verticalInput;

        private bool _isActionKey;

        public MovementState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
            Speed = character.WalkSpeed;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _horizontalInput = _verticalInput = 0.0f;
            Speed = Character.WalkSpeed;
        }

        public override void ProcessInput()
        {
            base.ProcessInput();
            _verticalInput = Input.GetAxisRaw("Vertical");
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _isActionKey = Input.GetButtonDown("Action");
        }

        public override void Update()
        {
            base.Update();
            Character.Move(Speed, _verticalInput, _horizontalInput);
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            
            if (_isActionKey)
            {
                if (Character.ClimbingTarget != null)
                {
                    Character.ActionQueue.Enqueue(Character.ClimbingState);
                    Vector3 pos = Character.ClimbingTarget.InverseTransformPoint(Character.Transform.position);
                    pos.z = 0;
                    pos = Character.ClimbingTarget.TransformPoint(pos);
                    StateMachine.ChangeState(new WalkToState(Character,StateMachine, pos, false));
                }
            }
        }
    }
}
