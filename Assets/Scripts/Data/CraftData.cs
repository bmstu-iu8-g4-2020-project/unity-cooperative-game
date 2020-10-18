using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class CraftCostData
{
    public Dictionary<ItemData, int> craftItems = new Dictionary<ItemData, int>();


    public GroupData craftNear;
}

/// <summary>
/// Parent data class for craftable items (items, constructions, plants...)
/// </summary>
public class CraftData : ScriptableObject
{
    [Header("--- CraftData ------------------")]
    public string id;

    [Header("Display")]
    public string title;

    public Sprite icon;

    [TextArea(3, 5)]
    public string description;

    [Header("Groups")]
    public GroupData[] groups;

    [Header("Crafting")]
    public bool craftable; //Can be crafted?


    public int craftQuantity = 1; //Does it craft more than 1?


    public GroupData
        craftNear; //Group of selectable required near the player to craft this (ex: fire source, water source)


    public ItemData[] craftItems; //Items needed to craft this

    // [Header("FX")] public AudioClip craft_sound;

    private static List<CraftData> _craftData = new List<CraftData>();

    public bool HasGroup(GroupData group)
    {
        foreach (GroupData agroup in groups)
        {
            if (agroup == group)
                return true;
        }

        return false;
    }

    public bool HasGroup(GroupData[] mgroups)
    {
        foreach (GroupData mgroup in mgroups)
        {
            foreach (GroupData agroup in groups)
            {
                if (agroup == mgroup)
                    return true;
            }
        }

        return false;
    }

    public ItemData GetItem()
    {
        if (this is ItemData)
            return (ItemData) this;
        return null;
    }

    public CraftCostData GetCraftCost()
    {
        CraftCostData cost = new CraftCostData();
        foreach (ItemData item in craftItems)
        {
            if (!cost.craftItems.ContainsKey(item))
                cost.craftItems[item] = 1;
            else
                cost.craftItems[item] += 1;
        }

        if (craftNear != null)
            cost.craftNear = craftNear;

        return cost;
    }

    public static void Load()
    {
        _craftData.Clear();
        _craftData.AddRange(ItemData.GetAll());
    }

    public static List<CraftData> GetAllInGroup(GroupData group)
    {
        List<CraftData> olist = new List<CraftData>();
        foreach (CraftData item in _craftData)
        {
            if (item.HasGroup(group))
                olist.Add(item);
        }

        return olist;
    }

    public static List<CraftData> GetAllCraftableInGroup(GroupData group)
    {
        List<CraftData> olist = new List<CraftData>();
        foreach (CraftData item in _craftData)
        {
            if (item.craftable && item.craftQuantity > 0 && item.HasGroup(group))
                olist.Add(item);
        }

        return olist;
    }

    public static CraftData Get(string id)
    {
        foreach (CraftData item in _craftData)
        {
            if (item.id == id)
                return item;
        }

        return null;
    }

    public static List<CraftData> GetAll()
    {
        return _craftData;
    }
}
