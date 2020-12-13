using UnityEngine;

namespace Entities.Player
{
    /// <summary>
    /// Keyboard controls and mouse buttons manager 
    /// </summary>
    public class PlayerControls : MonoBehaviour
    {
        [SerializeField]
        private KeyCode actionKey = KeyCode.F;


        [SerializeField]
        private KeyCode attackKey = KeyCode.Mouse0; //Can attack only in stealth(aiming) state


        [SerializeField]
        private KeyCode sprintKey = KeyCode.LeftShift;


        [SerializeField]
        private KeyCode stealthKey1 = KeyCode.LeftControl;


        [SerializeField]
        private KeyCode stealthKey2 = KeyCode.Mouse1;


        [SerializeField]
        private KeyCode push = KeyCode.Space;

        [SerializeField]
        private KeyCode camRotateLeft = KeyCode.Q;

        [SerializeField]
        private KeyCode camRotateRight = KeyCode.E;

        private Vector3 _move;
        private float _rotateCam;
        private bool _pressAttack;
        private bool _pressAction;
        private bool _pressSprint;
        private bool _holdSprint;
        private bool _pressStealth;
        private bool _holdStealth;
        private bool _pressPush;

        #region singleton

        public static PlayerControls Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning($"Removed duplicate singleton script on {gameObject.name}");
                Destroy(this);
            }
        }

        #endregion

        void Update()
        {
            _move = Vector3.zero;
            _rotateCam = 0f;
            _pressAttack = false;
            _pressAction = false;
            _pressSprint = false;
            _pressStealth = false;
            _holdSprint = false;
            _holdStealth = false;
            _pressPush = false;

            if (Input.GetKey(KeyCode.A))
                _move += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                _move += Vector3.right;
            if (Input.GetKey(KeyCode.W))
                _move += Vector3.forward;
            if (Input.GetKey(KeyCode.S))
                _move += Vector3.back;

            if (Input.GetKey(KeyCode.LeftArrow))
                _move += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                _move += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow))
                _move += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow))
                _move += Vector3.back;

            _move = _move.normalized * Mathf.Min(_move.magnitude, 1f);

            if (Input.GetKey(camRotateLeft))
                _rotateCam += -1f;
            if (Input.GetKey(camRotateRight))
                _rotateCam += 1f;

            if (Input.GetKeyDown(stealthKey1) || Input.GetKeyDown(stealthKey2))
                _pressStealth = true;
            if (Input.GetKeyDown(actionKey))
                _pressAction = true;
            if (Input.GetKeyDown(sprintKey))
                _pressSprint = true;
            if (Input.GetKeyDown(attackKey))
                _pressAttack = true;

            if (Input.GetKeyDown(push))
                _pressPush = true;

            if (Input.GetKey(sprintKey))
                _holdSprint = true;
            if (Input.GetKey(stealthKey1) || Input.GetKey(stealthKey2))
                _holdStealth = true;
        }

        public bool IsMoving() => _move.magnitude > 0.1f;

        public bool IsPressStealth() => _pressStealth;
        public bool IsHoldStealth() => _holdStealth;

        public bool IsPressAction() => _pressAction;

        public bool IsPressAttack() => _pressAttack;

        public bool IsPressSprint() => _pressSprint;
        public bool IsHoldSprint() => _holdSprint;

        public bool IsPressPush() => _pressPush;

        public Vector3 GetMove() => _move;

        public float GetRotateCam() => _rotateCam;
    }
}
