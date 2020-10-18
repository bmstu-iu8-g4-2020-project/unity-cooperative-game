using System.Collections;
using System.Collections.Generic;
using Actions;
using Gameplay;
using UnityEngine;
using UnityEngine.Serialization;


public enum ItemType
{
    Basic = 0,
    Consumable = 10,
    Equipment = 20,
}

public enum DurabilityType
{
    None = 0,
    UsageCount = 5, //Each use (like attacking or receiving hit) reduces durability, value is in use count
    UsageTime = 8, //Similar to spoilage, but only reduces while equipped, value is in game-hours
    Spoilage = 10, //Reduces over time, even when not in inventory, value is in game-hours
}

public enum EquipSlot
{
    None = 0,
    Hand = 10,
    Head = 20,
    Body = 30,
    Feet = 40,
    Slot5 = 50,
    Slot6 = 60,
    Slot7 = 70,
    Slot8 = 80,
    Slot9 = 90,
}

public enum EquipSide
{
    Default = 0,
    Right = 2,
    Left = 4,
}

/// <summary>
/// Data file for Items
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData", order = 2)]
public class ItemData : CraftData
{
    [Header("--- ItemData ------------------")]
    public ItemType type;


    [Header("Stats")]
    public DurabilityType durabilityType;

    public float durability = 0f; //0f means infinite, 1f per hour for consumable, 1f per hit for equipment
    public int maxCondition = 100;
    public float weight = 0.1f;
    public bool isStackable = true;
    public bool isDegradable = true;

    [Header("Stats Equip")]
    public EquipSlot equipSlot;

    public EquipSide equipSide;

    public bool weapon;
    public bool ranged;
    public int damage = 0;
    public int armor = 0;
    public float range = 1f;

    public BonusEffectData[] equipBonus;

    [Header("Stats Consume")]
    public int eatHp = 0;

    public int eatHunger = 0;
    public int eatThirst = 0;
    public int eatHappiness = 0;

    public BonusEffectData[] eatBonus;

    public float eatBonusDuration = 0f;

    [Header("Action")]
    public Action[] actions;

    [Header("Ref Data")]
    public ItemData containerData;

    [Header("Prefab")]
    public GameObject itemPrefab;

    public GameObject equippedPrefab;

    public GameObject projectilePrefab;

    private static List<ItemData> _itemData = new List<ItemData>(); //For looping
    private static Dictionary<string, ItemData> _itemDict = new Dictionary<string, ItemData>(); //Faster access

    public bool CanBeDropped()
    {
        return itemPrefab != null;
    }

    public bool HasDurability()
    {
        return durabilityType != DurabilityType.None && durability >= 0.1f;
    }

    //From 0 to 100
    public int GetDurabilityPercent(float currentDurability)
    {
        float perc = Mathf.Clamp01(currentDurability / durability);
        return Mathf.RoundToInt(perc * 100f);
    }

    public static void Load(string itemsFolder)
    {
        _itemData.Clear();
        _itemDict.Clear();
        _itemData.AddRange(Resources.LoadAll<ItemData>(itemsFolder));
        foreach (ItemData item in _itemData)
        {
            _itemDict.Add(item.id, item);
        }
    }

    public new static ItemData Get(string itemID)
    {
        if (itemID != null && _itemDict.ContainsKey(itemID))
            return _itemDict[itemID];
        return null;
    }

    public new static List<ItemData> GetAll()
    {
        return _itemData;
    }

    public static int GetEquipIndex(EquipSlot slot)
    {
        if (slot == EquipSlot.Hand)
            return 0;
        if (slot == EquipSlot.Head)
            return 1;
        if (slot == EquipSlot.Body)
            return 2;
        if (slot == EquipSlot.Feet)
            return 3;
        return -1;
    }

    public static EquipSlot GetEquipType(int index)
    {
        if (index == 0)
            return EquipSlot.Hand;
        if (index == 1)
            return EquipSlot.Head;
        if (index == 2)
            return EquipSlot.Body;
        if (index == 3)
            return EquipSlot.Feet;
        return EquipSlot.None;
    }

    public static ItemData FindByName(string itemName)
    {
        // itemName = itemName.ToLower();
        ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");

        if (itemData == null)
        {
            Debug.LogWarning($"Could not find \"{itemName}\". _item slot is empty.");
        }

        return itemData;
    }
}
