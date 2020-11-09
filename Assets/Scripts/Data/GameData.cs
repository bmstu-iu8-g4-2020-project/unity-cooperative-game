using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Generic game data (only one file)
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData", order = 0)]
public class GameData : ScriptableObject
{
    [Header("Game")]
    //A value of 1 means time follows real life time. Value of 24 means 1 hour of real time will be one day in game
    public float gameTimeMult = 24f;

    public int generatedLootQuantity = 1;

    // [Header("Day/Night")] 


    [Header("Music")]
    public AudioClip[] musicPlaylist;


    [Header("FX")]
    public GameObject itemTakeFX;

    public GameObject itemSelectFX;

    public GameObject itemMergeFX;

    public static GameData Get()
    {
        return TheData.Instance.GameData;
    }
}
