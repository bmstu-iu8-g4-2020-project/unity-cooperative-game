using System;
using Entities.PerTickAttribute;
using Mirror;
using UnityEngine;

namespace Entities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EntityStats))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(CombatActor))]
    public class Entity : NetworkBehaviour
    {
        public EntityStats Stats { get; private set; }
        public Health Health { get; private set; }
        public CombatActor CombatActor { get; private set; }

        public bool IsAlive => Health.Current > 0;

        private void Start()
        {
            Stats = GetComponent<EntityStats>();
            Health = GetComponent<Health>();
            CombatActor = GetComponent<CombatActor>();
        }
    }
}
