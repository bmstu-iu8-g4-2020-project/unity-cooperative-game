using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Mirror;
using States;
using UnityEngine;
using UnityEngine.UI;

public class Character : NetworkBehaviour
{
    #region Variables

    private Text _text; // todo delete

    private StateMachine _stateMachine;
    public WalkState WalkState { get; private set; }
    public StealthState StealthState { get; private set; }
    public SprintingState SprintState { get; private set; }
    public ClimbingState ClimbingState { get; private set; }

    public readonly Queue<State> actionQueue = new Queue<State>(); // TODO move to StateMachine?

    [field: SerializeField] public float WalkSpeed { get; private set; } = 10f;
    [field: SerializeField] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public float StealthSpeed { get; private set; } = 5f;

    public bool IsAiming { get; private set; } = false;
    public Transform ClimbingTarget { get; private set; } = null;
    [field: SerializeField] public float ClimbingDuration { get; private set; } = 2.0f;

    public CharacterController Controller { get; private set; }
    public Camera PlayerCamera { get; private set; }

    public Transform Transform { get; private set; }

    private Vector3 _forward;
    private Vector3 _right;
    private Vector3 _newPosition = Vector3.zero;
    private Quaternion _newRotation;

    private bool _isInterpolate = true;

    private const float SendDelay = 1 / 30f;

    #endregion


    #region MonoBehaviour Callbacks

    private void Awake()
    {
        _text = GameManager.Instance.DistText; // todo delete
        Transform = transform;

        Controller = GetComponent<CharacterController>();
        PlayerCamera = GameManager.Instance.GetSceneCamera();
    }

    // перелезание окна- телепортация. Отключать коллайдер.
    // но из-за интерполяции телепортация будет плавным перемещением.

    private void Start()
    {
        if (isLocalPlayer || isServer)
        {
            InvokeRepeating(nameof(NetworkUpdate), SendDelay, SendDelay);
        }

        GetComponent<FieldOfView>().enabled = isLocalPlayer; // TODO mb change place for this
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        gameObject.name = "LocalPlayer";
        PlayerCamera.transform.parent.GetComponent<CameraController>().SetTarget(Transform);

        _forward = PlayerCamera.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

        PlayerCamera.gameObject.SetActive(isLocalPlayer);

        // turn on light sources for the masking
        Transform.GetChild(3).gameObject.SetActive(true);
        Transform.GetChild(4).gameObject.SetActive(true);

        // FSM init
        _stateMachine = new StateMachine();

        WalkState = new WalkState(this, _stateMachine);
        StealthState = new StealthState(this, _stateMachine);
        SprintState = new SprintingState(this, _stateMachine);
        ClimbingState = new ClimbingState(this, _stateMachine);

        _stateMachine.Initialize(WalkState);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (ClimbingTarget != null)
            {
                _text.text = $"{ClimbingTarget.transform.InverseTransformPoint(Transform.position)}";
            }

            _stateMachine.CurrentState.ProcessInput();

            _stateMachine.CurrentState.Update();

            _stateMachine.CurrentState.MachineUpdate();
        }
        else if (isClient)
        {
            Transform.position = _isInterpolate ? Vector3.Lerp(Transform.position, _newPosition, 0.5f) : _newPosition;

            Transform.rotation = _newRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "ClimbTrigger":
                //TODO subscribe here?
                ClimbingTarget = other.transform.parent;
                break;
            case "WalkToTrigger":
                SwitchOnWalkTo(other.transform.GetChild(0).position);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "ClimbTrigger":
                ClimbingTarget = null;
                break;
        }
    }

    #endregion

    #region Networking

    private void NetworkUpdate()
    {
        if (isLocalPlayer)
        {
            CmdUpdateTransform(Transform.position, Transform.rotation);
        }

        if (isServer)
        {
            RpcSetTransform(_newPosition, _newRotation);
        }
    }


    [ClientRpc]
    private void RpcSetTransform(Vector3 newPosition, Quaternion newRotation)
    {
        if (isLocalPlayer) return;
        _newPosition = newPosition;
        _newRotation = newRotation;
    }

    [Command]
    private void CmdUpdateTransform(Vector3 newPosition, Quaternion newRotation)
    {
        _newPosition = newPosition;
        _newRotation = newRotation;
    }

    #endregion

    #region Methods

    [ClientCallback]
    public void Move(float speed, float verticalOffset, float horizontalOffset)
    {
        Vector3 velocity = _forward * verticalOffset + _right * horizontalOffset;
        velocity = velocity.normalized * (speed/* * Time.deltaTime*/);

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
        Vector3 direction = new Vector3(mousePos.x - screenPos.x, 0,
            mousePos.y - screenPos.y);
        direction = PlayerCamera.transform.TransformDirection(direction);
        direction.y = 0;
        Transform.forward = direction.normalized;
    }

    #endregion

    // todo adhere to single-responsibility principle? mb move this to StateMachine
    public void SwitchOnWalkTo(Vector3 destination, bool checkCollision = true)
    {
        _stateMachine.ChangeState(new WalkToState(this, _stateMachine, destination, checkCollision));
    }

    public void LookAtXZ(Vector3 target)
    {
        Vector3 targetPosition = new Vector3(target.x,
            Transform.position.y,
            target.z);
        Transform.LookAt(targetPosition);
    }
}
