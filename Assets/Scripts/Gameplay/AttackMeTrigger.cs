using System;
using Entities;
using Entities.Zombie;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class AttackMeTrigger : NetworkBehaviour
    {
        private void Start() => enabled = isServer;

        [FormerlySerializedAs("target")]
        [SerializeField]
        private Entity targetForAttack;

        [SerializeField]
        private float entryDistance = 2f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Zombie zombie)) return;
            zombie.AttackObstacle(targetForAttack.transform);
            targetForAttack.OnDying += entity => zombie.SetRoamingDestination(Vector3.back * entryDistance);
        }
    }
}
