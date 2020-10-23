using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// Manager script that will load all scriptable objects for use at runtime
/// </summary>
public class TheData : MonoBehaviour
{
    [field: SerializeField]
    public GameData GameData { get; private set; }

    [field: SerializeField]
    public PlayerData PlayerData { get; private set; }

    [Header("Resources")]
    public string itemsFolder = "Items";

    public string constructionsFolder = "Constructions";

    private static TheData _instance;

    #region singltone

    public static TheData Instance { get; private set; }

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
        
        // ItemData.Load(itemsFolder);
        // CraftData.Load();
    }

    #endregion
}
