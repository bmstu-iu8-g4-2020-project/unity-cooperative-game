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
    [RequireComponent(typeof(FieldOfView))]
    [RequireComponent(typeof(Player))]
    public class PlayerInteractionActor : MonoBehaviour
    {
        [field: SerializeField]
        public float InteractionRadius { get; private set; } = 3f;

        [field: SerializeField]
        public LayerMask InteractableLayerMask { get; private set; }

        [SerializeField]
        private uint maxInteractableCountForDetection = 10;

        private Collider[] _interactablesInRadius;
        private FieldOfView _fieldOfView;
        private Player _character;
        private Interactable _currentInteractionTarget;

        //Or Create Actor Controller that will be use Actor-Components in unity callbacks

        #region Unity Callbacks

        private void Awake()
        {
            _interactablesInRadius = new Collider[maxInteractableCountForDetection];
            _fieldOfView = GetComponent<FieldOfView>();
            _character = GetComponent<Player>();
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
                var interactable = GetNearestInteractableInRadiusOrNull();
                if (interactable != null)
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

        public Interactable GetNearestInteractableInRadiusOrNull()
        {
            Interactable nearest = null;
            float minDist = InteractionRadius;

            var size = Physics.OverlapSphereNonAlloc(transform.position, InteractionRadius, _interactablesInRadius,
                InteractableLayerMask);

            for (int i = 0; i < size; i++)
            {
                Transform target = _interactablesInRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) <= _fieldOfView.ViewAngle / 2)
                {
                    float dist = (transform.position - target.position).magnitude;
                    Interactable interactable = target.GetComponent<Interactable>();
                    if (interactable != null && dist < minDist && CanInteractWith(interactable))
                    {
                        minDist = dist;
                        nearest = interactable;
                    }
                }
            }

            return nearest;
        }
    }
}
