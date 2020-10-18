using Player;
using Player.States;
using UnityEngine;

namespace Gameplay
{
    //Для окна использовать заданную точку, для забора вычислять по нормали
    public class ClimbTarget : Interactable
    {
        [SerializeField]
        private Vector3 climbStartPoint; // TODO calculate by character collider radius

        public override void OnInteract(IInteractionActor interactionActor)
        {
            //Tell the actor to move to some point
        }
    }
}
