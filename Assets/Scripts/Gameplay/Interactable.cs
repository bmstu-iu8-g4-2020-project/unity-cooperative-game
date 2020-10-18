using Actions;
using Player;
using UnityEngine;

// use component on interactable object
namespace Gameplay
{
    public interface IInteractable
    {
        float Radius { get; }

        void OnInteract(IInteractionActor interactionActor);
        bool CanInteract();
        GameObject GetGameObject();
    }

    /// <summary>
    /// Interactable can contain action.
    /// </summary>
    public class Interactable : MonoBehaviour, IInteractable
    {
        [Header("Action")]
        public AAction action; //Action to be taken when interacting

        [field: SerializeField]
        public float Radius { get; private set; } = 3.0f;

        [field: SerializeField]
        public bool IsSelectable { get; } = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private void OnInteractWithPlayer(PlayerCharacter character)
        {
            Debug.Log($"Interacted with player {name}");
            if (action.CanDoAction(character, this))
            {
                Debug.Log($"Interacted with player {name}");
                action.DoAction(character, this);
            }
        }

        public virtual void OnInteract(IInteractionActor interactionActor)
        {
            var player = interactionActor as PlayerInteractionActor;
            if (player != null)
            {
                OnInteractWithPlayer(player.GetComponent<PlayerCharacter>());
            }
        }

        public bool CanInteract()
        {
            return action != null;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
