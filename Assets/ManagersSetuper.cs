using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class ManagersSetuper : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerControlsManager;

    [SerializeField]
    private GameObject eventSystem;

    public override void OnStartLocalPlayer()
    {
        
        base.OnStartLocalPlayer();
        Debug.Log($"OnStartLocalPlayer from ManagerSetuper");
    }
}
