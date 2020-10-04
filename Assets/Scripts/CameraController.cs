using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target;
    [SerializeField] [Range(0.01f, 1.0f)] private float smoothSpeed = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(-4, 4, -4);

    [SerializeField] private float maxShift = 2;
    [SerializeField] private float shiftSensitivity = 0.01f;
    private bool _isAiming = true;

    private Vector3 _forward;
    private Vector3 _right;

    private Vector3 _shift = Vector3.zero;

    private int _screenWidth;
    private int _screenHeight;

    private void Awake()
    {
        _forward = GameManager.Instance.GetSceneCamera().transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
    }

    private void Start()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
    }


    private void LateUpdate()
    {
        if (!_target) return;

        Vector3 desiredPos = _target.position + offset;

        // ~Interesting~ effect of camera jerking when aiming
        // if (Input.GetKey(KeyCode.Mouse1))
        // {
        //     var newShift = _forward * Input.GetAxis("Mouse Y") + _right * Input.GetAxis("Mouse X");
        //     newShift = Vector3.ClampMagnitude(newShift, maxShift);
        //     if (newShift != _shift && newShift != Vector3.zero)
        //     {
        //         _shift = newShift;
        //     }
        //     desiredPos += _shift * shiftSensitivity;
        // }

        if (_isAiming && Input.GetKey(KeyCode.Mouse1))
        {
            _shift = _right * (Input.mousePosition.x - _screenWidth / 2) +
                     _forward * (Input.mousePosition.y - _screenHeight / 2);
            _shift = Vector3.ClampMagnitude(_shift * shiftSensitivity, maxShift);

            desiredPos += _shift;
        }

        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothPos;
    }

    public void SetTarget(Transform target) => _target = target;


    private Vector3 VectorSigmoidInterpolation(Vector3 a, Vector3 b, float factor)
    {
        return new Vector3(Sigmoid(a.x, b.x, factor), Sigmoid(a.y, b.y, factor), Sigmoid(a.z, b.z, factor));
    }

    private static float Sigmoid(float a, float b, float t)
    {
        return (float) (a + (b - a) * (-2 * Mathf.Pow(t, 3) + 3 * Mathf.Pow(t, 2)));
    }

    public static Vector3 ClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double) max * (double) max) return v.normalized * max;
        else if (sm < (double) min * (double) min) return v.normalized * min;
        return v;
    }
}
