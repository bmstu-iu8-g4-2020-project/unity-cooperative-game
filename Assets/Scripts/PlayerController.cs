using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float covertSpeed = 5f;
    private bool _isSprint = false;
    private float _currentMoveSpeed;
    private CharacterController _controller;

    private Camera _playerCamera;

    private Vector3 _velocity;
    private Vector3 _newPosition = Vector3.zero;
    private Quaternion _newRotation;

    private Vector3 _forward;
    private Vector3 _right;

    private Transform _transform;

    private const float SendDelay = 1 / 30f;

    private bool _isInterpolate = true;

    private bool _canClimb = false;
    private Transform _climbingTarget;
    [SerializeField] private float climbingDuration = 1f;
    private bool _canChangeState = true;
    private bool _canControl = true;

    private void Awake()
    {
        _transform = transform;
        _currentMoveSpeed = walkSpeed;

        _controller = GetComponent<CharacterController>();
        _playerCamera = GameManager.Instance.GetSceneCamera();
    }

    // перелезание окна- телепортация. Отключать коллайдер.
    // но из-за интерполяции телепортация будет плавным перемещением.
    // TODO добавить state manager для персонажа?

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
        _playerCamera.transform.parent.GetComponent<CameraController>().SetTarget(_transform);

        _forward = _playerCamera.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

        _playerCamera.gameObject.SetActive(isLocalPlayer);

        // включение источников света для маски
        _transform.GetChild(3).gameObject.SetActive(true);
        _transform.GetChild(4).gameObject.SetActive(true);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (_canControl)
            {
                if (Input.anyKey)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        _isSprint = true;
                        _currentMoveSpeed = walkSpeed * sprintSpeedMultiplier;
                    }
                    else if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        _isSprint = false;
                        _currentMoveSpeed = walkSpeed;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftControl))
                    {
                        _currentMoveSpeed = covertSpeed;
                    }
                    else if (Input.GetKeyUp(KeyCode.LeftControl))
                    {
                        _currentMoveSpeed = walkSpeed;
                    }

                    Move();
                }

                if (Input.GetKey(KeyCode.Mouse1) && !_isSprint) FaceToMouse();

                if (_canClimb && _canChangeState && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(ClimbCoroutine());
                }
            }
        }
        else if (isClient)
        {
            _transform.position = _isInterpolate ? Vector3.Lerp(_transform.position, _newPosition, 0.5f) : _newPosition;

            _transform.rotation = _newRotation;
        }
    }

    private void NetworkUpdate()
    {
        if (isLocalPlayer)
        {
            CmdUpdateTransform(_transform.position, _transform.rotation);
        }

        if (isServer)
        {
            RpcSetTransform(_newPosition, _newRotation);
        }
    }

    [ClientCallback]
    private void Move()
    {
        _velocity = _forward * Input.GetAxisRaw("Vertical") + _right * Input.GetAxisRaw("Horizontal");
        _velocity = _velocity.normalized * (_currentMoveSpeed * Time.deltaTime);

        if (_velocity.magnitude == 0) return;
        _transform.forward = _velocity.normalized;

        _controller.Move(_velocity);
    }

    [ClientCallback]
    private void FaceToMouse()
    {
        Vector2 screenPos = _playerCamera.WorldToScreenPoint(_transform.position);
        Vector2 mousePos = Input.mousePosition;
        Vector3 direction = new Vector3(mousePos.x - screenPos.x, 0,
            mousePos.y - screenPos.y);
        direction = _playerCamera.transform.TransformDirection(direction);
        direction.y = 0;
        _transform.forward = direction.normalized;
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

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.CompareTag("ClimbTrigger"))
            {
                _canClimb = true;
                _climbingTarget = other.transform.parent;
            }
            else if (other.CompareTag("WalkToTrigger")) // TODO delete
            {
                StartCoroutine(WalkTo(other.transform.GetChild(0).position, true));
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.CompareTag("ClimbTrigger"))
            {
                _canClimb = false;
                _climbingTarget = null;
            }
        }
    }

    IEnumerator ClimbCoroutine()
    {
        if (!_climbingTarget) Debug.LogError("No target for climbing");
        var climbingTarget = _climbingTarget;

        _canChangeState = false;
        var collider = GetComponent<Collider>();
        collider.enabled = false;

        Vector3 relativePosition = climbingTarget.InverseTransformPoint(_transform.position);
        relativePosition.z = 0;
        
        yield return WalkTo(climbingTarget.TransformPoint(relativePosition), false);
        _canControl = false;
        LookAtXZ(climbingTarget.position);

        var radius = Vector3.Distance(relativePosition, Vector3.zero);
        float angle = Mathf.Acos(relativePosition.x / radius);
        float finalAngle = Mathf.Acos(-relativePosition.x / radius);
        var deltaAngle = Mathf.DeltaAngle(angle, finalAngle);
        float speed = deltaAngle / climbingDuration;
        float timeLeft = climbingDuration;
        while (timeLeft >= .00001f)
        {
            angle += speed * Time.deltaTime;
            timeLeft -= Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            _transform.position = climbingTarget.TransformPoint(new Vector3(x, y, 0));

            yield return null;
        }

        Vector3 curPos = _transform.position;
        curPos.y = climbingTarget.TransformPoint(relativePosition).y;
        _transform.position = curPos;

        collider.enabled = true;
        _canChangeState = true;
        _canControl = true;
    }

    IEnumerator WalkTo(Vector3 destination, bool checkCollision)
    {
        _canControl = false;
        LookAtXZ(destination);

        float lastCheckTime = 0f;
        Vector3 lastCheckPos = -_transform.position;
        float exitTime = 3.0f;
        float minDistance = 0.5f;
        while ((_transform.position - destination).magnitude >= 0.1f)
        {
            if (checkCollision)
            {
                _controller.Move((destination - _transform.position).normalized * (walkSpeed * Time.deltaTime));
                if (Time.time - lastCheckTime > exitTime)
                {
                    if ((_transform.position - lastCheckPos).magnitude < minDistance)
                    {
                        break;
                    }

                    lastCheckPos = _transform.position;
                    lastCheckTime = Time.time;
                }
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, destination, walkSpeed * Time.deltaTime);
            }

            yield return null;
        }

        _controller.Move(destination - _transform.position);

        _canControl = true;
    }


    void LookAtXZ(Vector3 target)
    {
        Vector3 targetPosition = new Vector3(target.x,
            _transform.position.y,
            target.z);
        _transform.LookAt(targetPosition);
    }
}
