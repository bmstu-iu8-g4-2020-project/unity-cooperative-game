using System;
using Entities.Player.States;
using Gameplay;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(fileName = "WindowClimb", menuName = "Data/Actions/Climb", order = 0)]
    public class ActionWindowClimb : AAction
    {
        public override void DoAction(Entities.Player.PlayerController character, Interactable interactable)
        {
            if (interactable == null) return;
            
            Vector3 pos = interactable.transform.InverseTransformPoint(character.transform.position);
            pos.x = Math.Sign(pos.x) * character.Controller.radius;
            pos.z = 0;
            pos = interactable.transform.TransformPoint(pos);
            //TODO mb make WalkTo state in this case unskipable
            character.StateMachine.AddStateToQueue(new WalkToState(character, character.StateMachine, pos, false));
            character.StateMachine.AddStateToQueue(new ClimbingState(character, character.StateMachine,
                interactable.transform));
            character.StateMachine.StartNextStateFromQueue();
        }

        public override bool CanDoAction(Entities.Player.PlayerController character, Interactable interactable) => true;
    }
}
