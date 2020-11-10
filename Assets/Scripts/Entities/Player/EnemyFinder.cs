using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;

namespace Entities.Player
{
    public class EnemyFinder : MonoBehaviour
    {
        private Collider _collider;
        private readonly List<Entity> _entitiesInRange = new List<Entity>();

        private void Awake() => _collider = GetComponent<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == transform.parent) return; //Exclude self collision
            var entity = other.gameObject.GetComponent<Entity>();
            if (entity != null) _entitiesInRange.Add(entity);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == transform.parent) return; //Exclude self collision
            var entity = other.gameObject.GetComponent<Entity>();
            if (entity != null) _entitiesInRange.Remove(entity);
        }

        public Entity[] GetNearest(uint n)
        {
            Entity[] result = new Entity[n];
            foreach (var entity in _entitiesInRange)
            {
                for (var i = 0; i < n; i++)
                {
                    if (result[i] == null || entity != null &&
                        Vector3.Distance(transform.parent.position, entity.transform.position) >
                        Vector3.Distance(transform.parent.position, result[i].transform.position))
                    {
                        result[i] = entity;
                    }
                }
            }

            return (from entity in result where entity != null select entity).ToArray();
        }
    }
}
