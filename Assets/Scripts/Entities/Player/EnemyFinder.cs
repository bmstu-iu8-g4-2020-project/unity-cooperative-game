using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities.Player
{
    public interface IEnemyFinder
    {
        Entity[] GetNearest(uint n);
        GameObject GameObject { get; }

        int EnemyInRange(); //TODO mb delete
    }

    public class EnemyFinder : MonoBehaviour, IEnemyFinder
    {
        private Collider _collider;

        private SortedSet<Entity> _entitiesInRange; //Entities sort by distance from this object

        private void Awake()
        {
            _entitiesInRange = new SortedSet<Entity>(new ByDistanceFrom(transform));
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == transform.parent) return; //Exclude self collision
            if (other.gameObject.TryGetComponent(out Entity entity))
            {
                _entitiesInRange.Add(entity);
                entity.OnDying += OnEnemyDie;
            }
        }

        private void OnEnemyDie(Entity entity)
        {
            _entitiesInRange.Remove(entity);
            entity.OnDying -= OnEnemyDie;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == transform.parent) return; //Exclude self collision
            if (other.gameObject.TryGetComponent(out Entity entity))
            {
                _entitiesInRange.Remove(entity);
            }
        }

        public Entity[] GetNearest(uint n) => _entitiesInRange.Take((int) n).ToArray();

        public GameObject GameObject => gameObject;
        public int EnemyInRange() => _entitiesInRange.Count;
    }

    public class ByDistanceFrom : IComparer<Entity>
    {
        private readonly Transform _thisTransform;
        public ByDistanceFrom(Transform thisTransform) => _thisTransform = thisTransform;

        public int Compare(Entity x, Entity y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var position = _thisTransform.position;
            return (int) Vector3.Distance(x.transform.position, position) -
                   (int) Vector3.Distance(y.transform.position, position);
        }
    }
}
