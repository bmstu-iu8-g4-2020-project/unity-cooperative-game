using System;
using Entities.Attributes;
using Entities.Player;
using Mirror;
using UI;
using UnityEngine;

namespace Entities.PerTickAttribute
{
    public class PerTickAttribute : NetworkBehaviour
    {
        [field: SerializeField]
        public int Max { get; protected set; } //TODO get from stats

        [SerializeField]
        private int basePerTick;

        [SerializeField]
        private int tickRate = 1;

        public PerTickAttribute overflowInto;
        public PerTickAttribute underflowInto;

        private Entity _entity;
        public event Action OnEmpty;

        public delegate void ChangeDelegate(int oldValue, int newValue);

        public event ChangeDelegate OnChange;

        public void OnChangeHook(int oldValue, int newValue)
        {
            OnChange?.Invoke(oldValue, newValue);
            if (isLocalPlayer) UIController.Instance.PerTickAttributesBarsUI.UpdateBar(GetType().Name, Percent());
        }

        [SyncVar(hook = nameof(OnChangeHook))]
        private int _current;

        public int PerTick => (int) (basePerTick * (1 - (ResistAttr?.GetModified() ?? 0)));
        public FloatAttribute ResistAttr { get; protected set; }

        public int Current
        {
            get => Mathf.Min(_current, Max);
            [Server]
            set
            {
                var emptyBefore = _current == 0;
                var old = _current;
                _current = Mathf.Clamp(value, 0, Max);
                if (isServer && isClient) OnChangeHook(old, _current);
                if (_current == 0 && !emptyBefore) OnEmpty?.Invoke();
            }
        }

        public float Percent() => Current != 0 && Max != 0 ? (float) Current / Max : 0;

        [Server]
        public void Recover()
        {
            if (!_entity.IsAlive) return;

            var next = Current + PerTick;

            Current = next;

            if (next < 0 && underflowInto != null)
                underflowInto.Current += next;
            else if (next > Max && overflowInto != null)
                overflowInto.Current += next - Max;
        }

        public override void OnStartServer()
        {
            _current = Max;

            _entity = GetComponent<Entity>();
            InvokeRepeating(nameof(Recover), tickRate, tickRate);
        }
    }
}
