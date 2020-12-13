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
    private readonly List<GameObject> _allPlayers = new List<GameObject>();

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

    private bool paused = false;
    private float speed_multiplier = 1f;


    #region singleton

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"Removed duplicate singleton script on {gameObject.name}");
            Destroy(this);
        }
    }

    #endregion

    public Camera GetCamera() => sceneCamera;

    public GameObject GetLocalPlayer() => _localPlayer;

    public void AddPlayer(GameObject player) => _allPlayers.Add(player);

    public GameObject[] GetAllPlayers() => _allPlayers.ToArray();

    public void SetLocalPlayer(GameObject player) => _localPlayer = player;

    public float GetGameTimeSpeed()
    {
        float gameSpeed = speed_multiplier * GameData.Get().gameTimeMult;
        return gameSpeed;
    }

    //Game hours per real time seconds
    public float GetGameTimeSpeedPerSec()
    {
        float hourToSec = GetGameTimeSpeed() / 3600f;
        return hourToSec;
    }
}
