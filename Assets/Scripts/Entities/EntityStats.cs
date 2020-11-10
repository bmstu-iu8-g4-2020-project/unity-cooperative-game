using System;
using Entities.Attributes;
using UnityEngine;

namespace Entities
{
    [DisallowMultipleComponent]
    public class EntityStats : MonoBehaviour
    {
        //коллекция аттрибутов
        //TODO make field serializable and change to MonoBehaviour
        [field: SerializeField]
        public IntAttribute MaxHealth { get;  private set;} = new IntAttribute(100);

        [field: SerializeField]
        public IntAttribute Attack { get;  private set;} = new IntAttribute(0);

        [field: SerializeField]
        public IntAttribute Defence { get;  private set;} = new IntAttribute(0);
    }
}
