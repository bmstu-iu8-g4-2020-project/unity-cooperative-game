using System;
using Entities.PerTickAttribute;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Entities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EntityStats))]
    [RequireComponent(typeof(Health))]
    public class Entity : NetworkBehaviour
    {
        public EntityStats Stats { get; private set; }
        public Health Health { get; private set; }

        public bool IsAlive => Health.Current > 0;

        private void Start()
        {
            Stats = GetComponent<EntityStats>();
            Health = GetComponent<Health>();
        }

        public virtual void Kill() => Destroy(gameObject);
    }
}
