using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    private CharacterController _controller;

    private Camera _playerCamera;

    private Vector3 _velocity;
    private Vector3 _newPosition;

    [SerializeField] private float rotationSpeed = 100f;
    private Quaternion _lookRotation;
    private Vector3 _direction;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _playerCamera = transform.GetChild(0).gameObject.GetComponent<Camera>();
        _playerCamera.gameObject.SetActive(isLocalPlayer);

        if (!isLocalPlayer)
        {
            //disable this component. It doesn't move the other player
            enabled = false;
            var mainCamera = Camera.main;
            if (!(mainCamera is null))
            {
                mainCamera.GetComponent<AudioListener>().enabled = false;
                mainCamera.GetComponent<Camera>().enabled = false;
                mainCamera.gameObject.SetActive(false);
            }
        }
    }

    [ClientRpc]
    private void RpcSetPosition(Vector3 newPosition)
    {
        if (isLocalPlayer) return;
        transform.position = newPosition;
    }

    [Command]
    private void CmdUpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        RpcSetPosition(newPosition);
    }

    [ClientRpc]
    private void RpcSetRotation(Quaternion newRotation)
    {
        if (isLocalPlayer) return;
        transform.rotation = newRotation;
    }

    [Command]
    private void CmdUpdateRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
        RpcSetRotation(newRotation);
    }

    [ClientCallback]
    private void LookAtMouse()
    {
        if (!isLocalPlayer) return;
        
        Vector2 positionOnScreen = _playerCamera.WorldToViewportPoint(transform.position);
        Debug.Log($"Player positionOnScreen: {positionOnScreen}");
        Vector2 mouseOnScreen = (Vector2) _playerCamera.ScreenToViewportPoint(Input.mousePosition);
        Debug.Log($"mouseOnScreen: {mouseOnScreen}");
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }


    void Update()
    {
        if (!isLocalPlayer) return;

        _newPosition = gameObject.transform.position;
        _velocity.x = Input.GetAxisRaw("Horizontal");
        _velocity.z = Input.GetAxisRaw("Vertical");
        _velocity = _velocity.normalized * (speed * Time.deltaTime);

        _controller.Move(_velocity);

        _newPosition = _controller.transform.position;

        if (isServer)
        {
            RpcSetPosition(_newPosition);
        }
        else if (isClient)
        {
            CmdUpdatePosition(_newPosition);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            LookAtMouse();
            if (isServer)
            {
                RpcSetRotation(_lookRotation);
            }
            else if (isClient)
            {
                CmdUpdateRotation(_lookRotation);
            }
        }
    }
}