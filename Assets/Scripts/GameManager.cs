using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game Manager Script
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private GameObject _localPlayer;

    [field: SerializeField]
    public Text StateText { get; set; }

    [field: SerializeField]
    public Text FPSText { get; set; }

    [field: SerializeField]
    public Text DistText { get; set; }

    [field: SerializeField]
    public UIContainerPanel UIContainerPanel { get; private set; }

    [field: SerializeField]
    public UIContainerPanel UIInventoryContainerPanel { get; private set; }

    #region singltone

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"Removed duplicate singltone script on {gameObject.name}");
            Destroy(this);
        }
    }

    #endregion

    public Camera GetCamera() => sceneCamera;

    public GameObject GetLocalPlayer()
    {
        return _localPlayer; // todo implement
    }

    public void SetLocalPlayer(GameObject player)
    {
        _localPlayer = player;
    }
}
