﻿using Actions;
using Player;
using UnityEngine;

namespace Gameplay
{

    /// <summary>
    /// Interactable can contain action.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [Header("Action")]
        public AAction action; //Action to be taken when interacting

        public Action[] actions;

        [field: SerializeField]
        public float Radius { get; private set; } = 2.0f;

        [field: SerializeField]
        public bool IsSelectable { get; private set; } = true;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private void OnInteractWithPlayer(PlayerCharacter character)
        {
            if (action == null)
            {
                Debug.Log($"Action doesn't set for {gameObject.name}");
            }

            if (action.CanDoAction(character, this))
            {
                Debug.Log($"Interacted with player {name}");
                action.DoAction(character, this);
            }
        }

        public void OnInteract(PlayerInteractionActor interactionActor)
        {
            var player = interactionActor as PlayerInteractionActor;
            if (player != null)
            {
                OnInteractWithPlayer(player.GetComponent<PlayerCharacter>());
            }
        }

        public void OnStopInteraction()
        {
            Debug.Log("Stop Interaction");
            action.StopAction();
        }

        public bool CanInteract() => action != null;
    }
}
