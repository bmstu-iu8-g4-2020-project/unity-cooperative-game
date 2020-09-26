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
    private Vector3 _lastPosition = Vector3.zero; // mb delete caching?
    private Vector3 _newPosition = Vector3.zero;
    private Quaternion _lastRotation;
    private Quaternion _newRotation;

    private Vector3 _forward;
    private Vector3 _right;

    private const float SendRate = 1 / 30f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (!isLocalPlayer) return;
        InvokeRepeating(nameof(NetworkUpdate), SendRate, SendRate);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        gameObject.name = "LocalPlayer";
        if (!(Camera.main is null)) Camera.main.transform.parent.GetComponent<CameraFollow>().target = transform;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer) return;

        _playerCamera = Camera.main;
        if (!(_playerCamera is null))
        {
            _forward = _playerCamera.transform.forward;
            _forward.y = 0;
            _forward = Vector3.Normalize(_forward);
            _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

            _playerCamera.gameObject.SetActive(isLocalPlayer);
        }
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
            Transform transform1 = transform;
            transform1.position = Vector3.Lerp(transform1.position, _newPosition, 0.5f);
            transform1.rotation = _newRotation;
        }
    }

    private void NetworkUpdate()
    {
        var transform1 = transform;
        if (isServer)
        {
            // todo better interpolation
            RpcSetTransform(transform1.position + _velocity * 0.1f, transform1.rotation);
        }
        else if (isClient)
        {
            CmdUpdateTransform(transform1.position + _velocity * 0.1f, transform1.rotation);
        }
    }

    [ClientCallback]
    private void Move()
    {
        _velocity = _forward * Input.GetAxisRaw("Vertical") + _right * Input.GetAxisRaw("Horizontal");
        _velocity = _velocity.normalized * (moveSpeed * Time.deltaTime);

        if (_velocity.magnitude == 0) return;
        transform.forward = _velocity.normalized;

        _controller.Move(_velocity);
        _lastPosition = transform.position;
    }

    [ClientCallback]
    private void FaceToMouse()
    {
        // TODO? fix allocation in update?
        Vector2 screenPos = _playerCamera.WorldToScreenPoint(_lastPosition);
        Vector2 mousePos = Input.mousePosition;
        Vector3 direction = new Vector3(mousePos.x - screenPos.x, 0,
            mousePos.y - screenPos.y);
        direction = _playerCamera.transform.TransformDirection(direction);
        direction.y = 0;
        transform.forward =direction.normalized;
        _lastRotation = transform.rotation;
    }

    // mb pass in function something lighter than quaternion
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
        // its not needed for the host
        // transform.position = newPosition;
        // transform.rotation = newRotation;
        RpcSetTransform(newPosition, newRotation);
    }
}