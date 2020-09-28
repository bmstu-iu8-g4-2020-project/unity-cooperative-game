using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private CharacterController _controller;

    private Camera _playerCamera;

    private Vector3 _velocity;
    private Vector3 _newPosition = Vector3.zero;
    private Quaternion _newRotation;

    private Vector3 _forward;
    private Vector3 _right;

    private Transform _transform;

    private const float SendDelay = 1 / 30f;

    private void Awake()
    {
        _transform = transform;

        _controller = GetComponent<CharacterController>();
        _playerCamera = SceneManager.Instance.GETSceneCamera();
    }

    private void Start()
    {
        // (isServer || isLocalPlayer) - works badly for host
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
        _playerCamera.transform.parent.GetComponent<CameraFollow>().SetTarget(_transform);
        
        _forward = _playerCamera.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

        _playerCamera.gameObject.SetActive(isLocalPlayer);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.anyKey) Move();
            if (Input.GetKey(KeyCode.Mouse1)) FaceToMouse();
        }
        else if (isClient)
        {
            _transform.position = Vector3.Lerp(_transform.position, _newPosition, 0.5f);
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
        _velocity = _velocity.normalized * (moveSpeed * Time.deltaTime);

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
        // if its not host
        // if (!isClient)
        // {
        //     _transform.position = newPosition;
        //     _transform.rotation = newRotation;
        // }
        _newPosition = newPosition;
        _newRotation = newRotation;
    }
}
