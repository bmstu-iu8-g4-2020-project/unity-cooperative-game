using System;
using Entities.Attributes;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Entities.PerTickAttribute
{
    public class PerTickAttribute : MonoBehaviour
    {
        [field: SerializeField]
        public int Max { get; protected set; }//TODO get from stats

        [SerializeField]
        private int basePerTick;

        [SerializeField]
        private int tickRate = 1;

        public PerTickAttribute overflowInto;
        public PerTickAttribute underflowInto;

        // [Header("Events")]
        public event UnityAction onEmpty;

        private int _current;

        public int PerTick => (int) (basePerTick * (1 - (ResistAttr?.GetModified() ?? 0)));

        public FloatAttribute ResistAttr { get; protected set; }

        public int Current
        {
            get => Mathf.Min(_current, Max);
            set
            {
                var emptyBefore = _current == 0;
                _current = Mathf.Clamp(value, 0, Max);
                if (_current == 0 && !emptyBefore) onEmpty?.Invoke();
                UIController.Instance.PerTickAttributesBarsUI.UpdateBar(this, Percent());
            }
        }

        public float Percent() => Current != 0 && Max != 0 ? (float) Current / Max : 0;

        public void Recover()
        {
            //todo if (Player is Alive)
            // {
            var next = Current + PerTick;

            Current = next;

            if (next < 0 && underflowInto != null)
                underflowInto.Current += next;
            else if (next > Max && overflowInto != null)
                overflowInto.Current += next - Max;
            // }
        }

        protected virtual void Start()
        {
            _current = Max;
            InvokeRepeating(nameof(Recover), tickRate, tickRate);
        }
    }
}
