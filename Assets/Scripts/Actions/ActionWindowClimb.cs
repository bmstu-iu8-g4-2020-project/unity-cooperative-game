using System;
using Gameplay;
using Player;
using Player.States;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(fileName = "Action", menuName = "Data/Actions/Climb", order = 0)]
    public class ActionWindowClimb : AAction
    {
        public override void DoAction(PlayerCharacter character, Interactable interactable)
        {
            if (interactable == null) return;

            Vector3 startPoint = character.Transform.position;
            startPoint.x = /*Math.Sign(startPoint.x) * */character.Controller.radius;

            Vector3 pos = interactable.transform.InverseTransformPoint(startPoint);
            pos.z = 0;
            pos = interactable.transform.TransformPoint(pos);
            character.StateMachine.AddStateToQueue(new WalkToState(character, character.StateMachine, pos, false));
            character.StateMachine.AddStateToQueue(new ClimbingState(character, character.StateMachine,
                interactable.transform));
            character.StateMachine.StartNextStateFromQueue();
        }

        public override bool CanDoAction(PlayerCharacter character, Interactable interactable)
        {
            return true;
        }
    }
}
