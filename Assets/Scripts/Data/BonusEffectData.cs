﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public enum BonusType
{
    None = 0,

    SpeedBoost = 5, //Value in percentage
    AttackBoost = 7, //Value in percentage
    ArmorBoost = 8, //Value in percentage

    HealthUp = 10, //Value in amount per game-hour
    HungerUp = 11, //Value in amount per game-hour
    ThirstUp = 12, //Value in amount per game-hour
    HappyUp = 13, //Value in amount per game-hour

    Invulnerable = 20, //In percentage, so 0.5 is half damage, 1 is no damage
}

/// <summary>
/// Data file bonus effects (ongoing effect applied to the character when equipping an item)
/// </summary>
[CreateAssetMenu(fileName = "BonusEffect", menuName = "Data/BonusEffect", order = 7)]
public class BonusEffectData : ScriptableObject
{
    public string effectID;

    public BonusType type;
    public float value;


    public static BonusType GetAttributeBonusType(AttributeType type)
    {
        if (type == AttributeType.Health)
            return BonusType.HealthUp;
        if (type == AttributeType.Hunger)
            return BonusType.HungerUp;
        if (type == AttributeType.Thirst)
            return BonusType.ThirstUp;
        if (type == AttributeType.Happiness)
            return BonusType.HappyUp;
        return BonusType.None;
    }
}
