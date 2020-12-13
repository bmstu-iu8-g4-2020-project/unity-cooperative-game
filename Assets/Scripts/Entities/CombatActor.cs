using System;
using Entities.Player;
using Mirror;
using UnityEngine;

namespace Entities
{
    public class CombatActor : NetworkBehaviour
    {
        [SerializeField]
        private EnemyFinder enemyFinder;

        protected IEnemyFinder _enemyFinder;

        protected float Cooldown { get; set; } = 2;
        protected float CooldownTimer;

        public virtual bool CanAttack() => CooldownTimer <= 0;

        private void Damage(Entity target, int amount)
        {
            if (!target.IsAlive) return;
            if (!isServer) target.CmdTakeDamage(amount);
            else target.TakeDamage(amount);
        }

        public virtual bool TryAttack(Entity target, int amount)
        {
            if (!CanAttack()) return false;

            Damage(target, amount);
            CooldownTimer = Cooldown;
            return true;
        }

        public void StartCooldown() => CooldownTimer = Cooldown;

        private void Start() => _enemyFinder = enemyFinder;

        protected virtual void Update()
        {
            if (CooldownTimer >= 0) CooldownTimer -= Time.deltaTime;
        }
    }
}
