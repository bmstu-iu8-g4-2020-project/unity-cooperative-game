using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _target;
    [SerializeField] [Range(0.01f, 1.0f)] private float smoothSpeed = 0.2f;
    public Vector3 offset;

    private void LateUpdate()
    {
        if (_target)
        {
            Vector3 desiredPos = _target.position + offset;
            Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.position = smoothPos;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private Vector3 VectorSigmoidInterpolation(Vector3 a, Vector3 b, float factor)
    {
        return new Vector3(Sigmoid(a.x, b.x, factor), Sigmoid(a.y, b.y, factor), Sigmoid(a.z, b.z, factor));
    }

    private static float Sigmoid(float a, float b, float t)
    {
        return (float) (a + (b - a) * (-2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2)));
    }
}
