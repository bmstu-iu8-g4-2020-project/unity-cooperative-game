using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    public Transform target;
    [Range(0.01f,1.0f)]
    public float smoothSpeed = 0.12f;
    public Vector3 offset;
    

    //Todo maybe move this functional to player
    private void LateUpdate()
    {
        if (!isClient) return;
        if (target)
        {
            Vector3 desiredPost = target.position + offset;
            Vector3 smoothPost = Vector3.Lerp(transform.position, desiredPost, smoothSpeed);
            transform.position = smoothPost;
        }
    }
}