using System;
using Gameplay;
using Player.States;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Entity can initiate interaction
    /// </summary>
    public interface IInteractionActor
    {
        float InteractionRadius { get; }
        LayerMask InteractableLayerMask { get; }
        void InteractWith(IInteractable interactable);
        bool CanInteractNow();
        bool CanInteractWith(IInteractable interactable);
        IInteractable GetNearestInteractableInRadiusOrNull();
        GameObject GetGameObject();
    }
    //При вводе с клавы выбирать тут ближайший объект для автоматического действия
    //При выборе объекта мышью

    /// <summary>
    /// Player component for initiate interaction
    /// </summary>
    [RequireComponent(typeof(FieldOfView))]
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerInteractionActor : MonoBehaviour, IInteractionActor
    {
        [field: SerializeField]
        public float InteractionRadius { get; private set; } = 3f;

        [field: SerializeField]
        public LayerMask InteractableLayerMask { get; private set; }

        [SerializeField]
        private uint maxInteractableCountForDetection = 10;

        private Collider[] _interactablesInRadius;
        private FieldOfView _fieldOfView;
        private PlayerCharacter _character;

        //Or Create Actor Controller that will be use Actor-Components in unity callbacks

        #region Unity Callbacks

        private void Awake()
        {
            _interactablesInRadius = new Collider[maxInteractableCountForDetection];
            _fieldOfView = GetComponent<FieldOfView>();
            _character = GetComponent<PlayerCharacter>();
        }

        private void Start()
        {
            PlayerControlsMouse.Instance.OnClickObject += OnClickObject;
        }

        private void OnDestroy()
        {
            PlayerControlsMouse.Instance.OnClickObject -= OnClickObject;
        }

        private void OnClickObject(Interactable interactable)
        {
            if (!interactable.IsSelectable) return;
            Debug.Log($"{interactable.gameObject.name} selected");
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

            //Todo highlights nearest interactable? Do it in SlowUpdate anyway
        }

        #endregion

        public void InteractWith(IInteractable interactable)
        {
            //Toggle highlights for object if not
            interactable.OnInteract(this);
        }

        public bool CanInteractNow()
        {
            return _character.StateMachine.CurrentState.GetType().IsSubclassOf(typeof(MovementState));
        }

        public bool CanInteractWith(IInteractable interactable)
        {
            if (interactable == null) return false;
            var dist = Vector3.Distance(transform.position, interactable.GetGameObject().transform.position);
            return CanInteractNow() && dist <= InteractionRadius && dist <= interactable.Radius &&
                   interactable.CanInteract();
        }

        public IInteractable GetNearestInteractableInRadiusOrNull()
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

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
