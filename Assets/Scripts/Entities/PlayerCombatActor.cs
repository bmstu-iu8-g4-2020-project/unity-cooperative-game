using System;
using Data.Items;
using Entities.Player;
using Entities.Player.States;
using UnityEngine;

namespace Entities
{
    public class PlayerCombatActor : MonoBehaviour
    {
        private Player.PlayerController _player;

        [SerializeField]
        private EnemyFinder enemyFinder;

        private bool CanAttack() => Player.PlayerController.LocalPlayer.StateMachine.CurrentState is StealthState;

        public static void Damage(Entity target, int amount)
        {
            if (!target.IsAlive) return;
            var amountWithResist = Math.Max(amount - target.Stats.Defence.GetModified(), 1);
            target.Health.Current -= amountWithResist;
            Debug.Log($"Damage {target.name}: -{amountWithResist}hp");
        }

        private void Awake() => _player = GetComponent<Player.PlayerController>();

        private float _cooldownTimer = 0;
        private HandItem _handItem;

        private void Update()
        {
            //TODO push on space
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            if (!PlayerControls.Instance.IsPressAttack() || !CanAttack()) return;

            if (!_player.Equipment.TryGetItemInMainHand(out _handItem)) return;

            _cooldownTimer = _handItem.CooldownTime;
            foreach (var entity in enemyFinder.GetNearest(_handItem.TargetsPerHit))
            {
                Damage(entity, _player.Stats.Attack.GetModified());
            }
        }
    }
}
