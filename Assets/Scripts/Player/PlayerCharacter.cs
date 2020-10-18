using System;
using Data;
using Gameplay;
using Mirror;
using Player.States;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main character script
    /// </summary>
    public class PlayerCharacter : MonoBehaviour
    {
        #region Variables

        private PlayerData _data;

        [SerializeField]
        private AttributeData[] attributes;

        public bool IsAiming { get; private set; } = false;

        public StateMachine StateMachine;
        public WalkState WalkState { get; private set; }
        public StealthState StealthState { get; private set; }
        public SprintingState SprintState { get; private set; }


        public CharacterController Controller { get; private set; }
        public Camera PlayerCamera { get; private set; }

        public Transform Transform { get; private set; }

        private Vector3 _forward;
        private Vector3 _right;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            Transform = transform;

            Controller = GetComponent<CharacterController>();
            PlayerCamera = GameManager.Instance.GetCamera();
        }

        private void Start()
        {
            _forward = PlayerCamera.transform.forward;
            _forward.y = 0;
            _forward = Vector3.Normalize(_forward);
            _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

            // FSM init
            StateMachine = new StateMachine();

            WalkState = new WalkState(this, StateMachine);
            StealthState = new StealthState(this, StateMachine);
            SprintState = new SprintingState(this, StateMachine);

            StateMachine.Initialize(WalkState);
        }

        void Update()
        {
            // ResolveAttributeEffects(); // todo implement

            StateMachine.CurrentState.Tick();

            StateMachine.CurrentState.MachineUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "WalkToTrigger":
                    var destination = other.transform.GetChild(0).position;
                    StateMachine.ChangeState(new WalkToState(this, StateMachine, destination, true));
                    break;
            }
        }

        #endregion


        #region Methods

        [ClientCallback]
        public void Move(float speed, float verticalOffset, float horizontalOffset)
        {
            Vector3 velocity = _forward * horizontalOffset + _right * verticalOffset;
            velocity = velocity.normalized * (speed /* * Time.deltaTime*/);

            if (velocity.magnitude != 0)
            {
                Transform.forward = velocity.normalized;
            }

            Controller.SimpleMove(velocity);
        }

        [ClientCallback]
        public void FaceToMouse()
        {
            Vector2 screenPos = PlayerCamera.WorldToScreenPoint(Transform.position);
            Vector2 mousePos = Input.mousePosition;
            Vector3 direction = new Vector3(mousePos.x - screenPos.x, 0, mousePos.y - screenPos.y);
            direction = PlayerCamera.transform.TransformDirection(direction);
            direction.y = 0;
            Transform.forward = direction.normalized;
        }

        public void LookAtXZ(Vector3 target)
        {
            Vector3 targetPosition = new Vector3(target.x,
                Transform.position.y,
                target.z);
            Transform.LookAt(targetPosition);
        }

        private void ResolveAttributeEffects()
        {
            float gameSpeed = 1f; // TODO get from GameManager from GameData

            //Update Attributes
            foreach (AttributeData attr in attributes)
            {
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}
