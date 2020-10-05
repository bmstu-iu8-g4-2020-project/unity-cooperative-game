using UnityEngine;

namespace States
{
    public class ActionState : State
    {
        protected bool IsExit;

        private bool _isStealth = false;

        public ActionState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
            IsExit = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            IsExit = false;
        }

        public override void ProcessInput()
        {
            base.ProcessInput();

            _isStealth = Input.GetButtonDown("Stealth");
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (_isStealth)
            {
                Character.ActionQueue.Clear();
                StateMachine.ChangeState(Character.StealthState);
            }
            else if (IsExit)
            {
                if (Character.ActionQueue.Count != 0)
                {
                    StateMachine.ChangeState(Character.ActionQueue.Dequeue());
                }
                else
                {
                    StateMachine.ChangeState(Character.WalkState);
                }
            }
        }
    }
}
