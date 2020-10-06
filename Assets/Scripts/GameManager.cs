using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [field: SerializeField] public Text StateText { get; private set; }
    [field: SerializeField] public Text FPSText { get; private set; }
    [field: SerializeField] public Text DistText { get; private set; }


    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    public Camera GetSceneCamera() => sceneCamera;
    
    

}
