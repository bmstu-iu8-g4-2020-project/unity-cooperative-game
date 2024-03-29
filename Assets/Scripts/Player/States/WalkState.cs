﻿namespace Player.States
{
    public class WalkState : MovementState
    {
        public WalkState(PlayerCharacter character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (PlayerControls.Instance.IsPressStealth())
            {
                StateMachine.ChangeState(Character.StealthState);
            }
            else if (PlayerControls.Instance.IsPressSprint())
            {
                StateMachine.ChangeState(Character.SprintState);
            }
        }
    }
}
