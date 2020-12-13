using Entities.Player.States;
using Gameplay;
using UnityEngine;

namespace Entities.Player
{
    //При вводе с клавы выбирать тут ближайший объект для автоматического действия
    //При выборе объекта мышью

    /// <summary>
    /// Player component for initiate interaction
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractionActor : MonoBehaviour
    {
        [field: SerializeField]
        public float InteractionRadius { get; private set; } = 3f;

        [field: SerializeField]
        public LayerMask InteractableLayerMask { get; private set; }

        [SerializeField]
        private uint maxInteractableCountForDetection = 10;

        [SerializeField]
        private FieldOfViewPlayer fieldOfView;

        private Collider[] _interactablesInRadius;
        private PlayerController _character;
        private Interactable _currentInteractionTarget;

        //Or Create Actor Controller that will be use Actor-Components in unity callbacks

        #region Unity Callbacks

        private void Awake()
        {
            _interactablesInRadius = new Collider[maxInteractableCountForDetection];
            _character = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            PlayerControlsMouse.Instance.OnClickObject += OnClickObject;
        }

        private void OnDisable()
        {
            PlayerControlsMouse.Instance.OnClickObject -= OnClickObject;
        }

        private void Update()
        {
            if (PlayerControls.Instance.IsPressAction() && CanInteractNow())
            {
                if (TryGetNearestInteractableInRadius(out Interactable interactable))
                {
                    InteractWith(interactable);
                }
            }

            if (_currentInteractionTarget != null && !CanInteractWith(_currentInteractionTarget))
            {
                StopInteraction();
            }

            //Todo highlights nearest interactable? Do it in SlowUpdate anyway
        }

        #endregion

        private void OnClickObject(Interactable interactable)
        {
            if (interactable == null || !interactable.IsSelectable) return;
            if (CanInteractWith(interactable) && CanSelectNow())
            {
                Debug.Log($"{interactable.gameObject.name} selected");
                InteractWith(interactable);
            }
        }

        public void InteractWith(Interactable interactable)
        {
            _currentInteractionTarget = interactable;
            //Toggle highlights for object
            interactable.OnInteract(this);
        }

        private void StopInteraction()
        {
            _currentInteractionTarget.OnStopInteraction();
            _currentInteractionTarget = null;
        }

        public bool CanSelectNow() =>
            !_character.StateMachine.CurrentState.GetType().IsSubclassOf(typeof(StealthState));

        public bool CanInteractNow() =>
            _character.StateMachine.CurrentState.GetType().IsSubclassOf(typeof(MovementState));

        public bool CanInteractWith(Interactable interactable)
        {
            if (interactable == null) return false;
            var dist = Vector3.Distance(transform.position, interactable.gameObject.transform.position);
            return CanInteractNow() && dist <= InteractionRadius && dist <= interactable.Radius &&
                   interactable.CanInteract();
        }

        public bool TryGetNearestInteractableInRadius(out Interactable nearest)
        {
            bool res = false;
            nearest = null;
            float minDist = InteractionRadius;

            var size = Physics.OverlapSphereNonAlloc(transform.position, InteractionRadius, _interactablesInRadius,
                InteractableLayerMask);

            for (int i = 0; i < size; i++)
            {
                Transform target = _interactablesInRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) <= fieldOfView.ViewAngle / 2)
                {
                    float dist = (transform.position - target.position).magnitude;
                    if (target.TryGetComponent(out Interactable interactable) && dist < minDist &&
                        CanInteractWith(interactable))
                    {
                        minDist = dist;
                        nearest = interactable;
                        res = true;
                    }
                }
            }

            return res;
        }
    }
}
