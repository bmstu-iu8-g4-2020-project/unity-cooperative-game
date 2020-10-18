using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A group doesnt do anything by itself but it serves to reference objects as a group instead of referencing them one by one.
/// Ex: all tools that can cut trees could be in the group 'CutTree' and trees would have a requirement that need the player to hold a 'CutTree' item.
/// </summary>
[CreateAssetMenu(fileName = "GroupData", menuName = "Data/GroupData", order = 1)]
public class GroupData : ScriptableObject
{
    public string title;
    public Sprite icon;
}
