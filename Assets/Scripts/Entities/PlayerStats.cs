using System;
using Entities.Attributes;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class PlayerStats : EntityStats
    {
        [field: SerializeField]
        public FloatAttribute HungerResist { get; private set; } = new FloatAttribute(0.1f);

        [field: SerializeField]
        public FloatAttribute TemperatureResist { get; private set; } = new FloatAttribute(0.1f);

        [field: SerializeField]
        public FloatAttribute ThirstResist { get; private set; } = new FloatAttribute(0.1f);

        private void Awake()
        {
            
        }
    }
}
