using System;
using Data;
using Data.Items;
using Entities.PerTickAttribute;
using Entities.Player;
using Entities.Player.States;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Entities
{
    public class PlayerCombatActor : MonoBehaviour
    {
        private Player.Player _player;

        [SerializeField]
        private EnemyFinder enemyFinder;

        private bool CanAttack() => Player.Player.LocalPlayer.StateMachine.CurrentState is StealthState;

        public void Damage(Entity target, int amount)
        {
            if (!target.IsAlive) return;
            var amountWithResist = Math.Max(amount - target.Stats.Defence.GetModified(), 1);
            target.Health.Current -= amountWithResist;
            Debug.Log($"Damage {target.name}: -{amountWithResist}hp");
        }

        private void Awake() => _player = GetComponent<Player.Player>();

        private float _cooldownTimer = 0;
        private HandItem _handItem;

        private void Update()
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }
            if (!PlayerControls.Instance.IsPressAttack() || !CanAttack()) return;

            _handItem = _player.Equipment.GetItemInMainHandOrNull();
            if (_handItem == null) return;

            _cooldownTimer = _handItem.CooldownTime;
            foreach (var entity in enemyFinder.GetNearest(_handItem.TargetsPerHit))
            {
                Damage(entity, _player.Stats.Attack.GetModified());
            }
        }
    }
}
