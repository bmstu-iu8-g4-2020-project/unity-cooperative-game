using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    // find all duplicates in list
    public static List<T2> FindDuplicates<T, T2>(this List<T> list, Func<T, T2> keySelector)
    {
        return list.GroupBy(keySelector)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key).ToList();
    }

    //Hash that same on all machines
    public static int GetStableHashCode(this string text)
    {
        unchecked
        {
            var hash = 23;
            foreach (var c in text)
                hash = hash * 31 + c;
            return hash;
        }
    }

    public static void LookAtXZ(this Transform transform, Vector3 target)
    {
        var targetPosition = new Vector3(target.x,
            transform.position.y,
            target.z);
        transform.LookAt(targetPosition);
    }
}
