using Entities.Player;
using Gameplay;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(fileName = "OpenDoor", menuName = "Data/Actions/OpenDoor", order = 0)]
    public class ActionOpenDoor : AAction
    {
        private static readonly int IsOpen = Animator.StringToHash("isOpen");

        public override void DoAction(PlayerController character, Interactable interactable)
        {
            if (interactable.transform.parent.TryGetComponent(out Animator animator) && !animator.IsPlaying())
                animator.SetBool(IsOpen, !animator.GetBool(IsOpen));
        }
    }
}
