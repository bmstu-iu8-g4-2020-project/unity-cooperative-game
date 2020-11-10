using Data;
using UnityEngine;

/// <summary>
///     Manager script that will load all scriptable objects for use at runtime
/// </summary>
public class TheData : MonoBehaviour
{
    private static TheData _instance;

    [Header("Resources")]
    public string itemsFolder = "Items";

    public string constructionsFolder = "Constructions";

    [field: SerializeField]
    public GameData GameData { get; private set; }

    [field: SerializeField]
    public PlayerData PlayerData { get; private set; }

    #region singleton

    public static TheData Instance { get; private set; }

    private void Awake()
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

        // ItemData.Load(itemsFolder);
        // CraftData.Load();
    }

    #endregion
}
