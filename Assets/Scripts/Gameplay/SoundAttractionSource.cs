using System;
using System.Collections.Generic;
using Entities;
using Entities.Player;
using UnityEngine;

namespace Gameplay
{
    public class SoundAttractionSource : MonoBehaviour
    {
        [field: SerializeField]
        public float Radius { get; set; } = 5f;

        [SerializeField]
        protected LayerMask _mask;

        private static int _maxListenersCount = 1000;

        private readonly Collider[] _listeners = new Collider[_maxListenersCount];

        public Actor[] FindListeners()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, Radius, _listeners, _mask);
            List<Actor> actors = new List<Actor>();

            foreach (var el in _listeners)
            {
                if (el.TryGetComponent(out Actor actor)) actors.Add(actor);
            }

            return actors.ToArray();
        }
    }
}
