﻿using System;
using Entities.Attributes;
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

        public event Action OnEmpty;

        public delegate void ChangeDelegate(int oldValue, int newValue);

        public event ChangeDelegate OnChange;

        [ClientRpc]
        public void RpcOnChange(int oldValue, int newValue)
        {
            OnChange?.Invoke(oldValue, newValue);
            if (isLocalPlayer) UIController.Instance.PerTickAttributesBarsUI.UpdateBar(GetType().Name, Percent());
        }

        [SyncVar]
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
                if (_current == 0 && !emptyBefore) OnEmpty?.Invoke();
                else RpcOnChange(old, _current);
            }
        }

        public float Percent() => Current != 0 && Max != 0 ? (float) Current / Max : 0;

        [Server]
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

        public override void OnStartServer()
        {
            _current = Max;
            InvokeRepeating(nameof(Recover), tickRate, tickRate);
        }
    }
}