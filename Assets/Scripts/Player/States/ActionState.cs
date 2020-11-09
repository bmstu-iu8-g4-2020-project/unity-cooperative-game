namespace Player.States
{
    /// <summary>
    /// The action the character is doing
    /// </summary>
    public class ActionState : State
    {
        protected bool IsExit;


        public ActionState(PlayerCharacter character, StateMachine stateMachine) : base(character, stateMachine)
        {
            IsExit = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            IsExit = false;
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (PlayerControls.Instance.IsHoldStealth())
            {
                StateMachine.ClearQueue();
                StateMachine.ChangeState(Character.StealthState);
            }
            else if (IsExit)
            {
                if (StateMachine.QueueEmpty())
                {
                    StateMachine.ChangeState(Character.WalkState);
                }
                else
                {
                    StateMachine.StartNextStateFromQueue();
                }
            }
        }
    }
}
