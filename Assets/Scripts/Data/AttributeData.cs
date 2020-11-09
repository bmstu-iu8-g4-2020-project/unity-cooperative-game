using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public enum AttributeType
{
    None = 0,
    Health = 2,
    Happiness = 4,
    Hunger = 6,
    Thirst = 8,
    Xp = 10,
    Attribute6 = 60,
    Attribute7 = 70,
    Attribute8 = 80,
    Attribute9 = 90,
}

/// <summary>
/// Attribute data (health, hunger, thirst, etc)
/// </summary>
[CreateAssetMenu(fileName = "AttributeData", menuName = "Data/AttributeData", order = 11)]
public class AttributeData : ScriptableObject
{
    public AttributeType type;
    public string title;

    [Space(5)]
    public float startValue = 100f; //Starting value
    
    public float maxValue = 100f; //Maximum value

    public float valuePerHour = -100f; //How much is gained (or lost) per in-game hour

    [Header("When reaches zero")]
    public float depleteHpLoss = -100f; //Per hour

    public float depleteMoveMult = 1f; //1f = normal speed
}
