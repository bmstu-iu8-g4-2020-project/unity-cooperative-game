using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;

namespace Data
{
    /// <summary>
    ///     Items static data
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Items/ItemData", order = 2)]
    public class ItemData : ScriptableObject
    {
        //Hash of item -> item. Hash is used for transmission over the network
        private static Dictionary<int, ItemData> cache;

        [Header("Stats")]
        public Sprite icon;

        public float weight = 0.1f;
        public int maxCondition = 100;
        public bool isStackable = true;
        public bool isDegradable;

        [TextArea(1, 30)]
        public string description;

        [Header("Action")]
        public Action[] actions; //Right-click actions 

        [Header("3D Representation")]
        public GameObject itemPrefab;

        public GameObject equippedPrefab;

        public static Dictionary<int, ItemData> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    // get all ItemDatas in resources
                    var items = Resources.LoadAll<ItemData>("");

                    // check for duplicates, then add to cache
                    var duplicates = items.ToList().FindDuplicates(item => item.name);
                    if (duplicates.Count == 0)
                        cache = items.ToDictionary(item => item.name.GetStableHashCode(), item => item);
                    else
                        foreach (var duplicate in duplicates)
                            Debug.LogError("Resources folder contains multiple ItemDatas with the name " + duplicate);
                }

                return cache;
            }
        }

        public static ItemData FindByName(string itemName)
        {
            // itemName = itemName.ToLower();
            var itemData = Resources.Load<ItemData>($"Items/{itemName}");

            if (itemData == null) Debug.LogWarning($"Could not find \"{itemName}\". _item slot is empty.");

            return itemData;
        }
    }
}
