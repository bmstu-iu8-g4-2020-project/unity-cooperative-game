using System;
using Entities.Player;
using UnityEngine;

namespace Entities
{
    public class CombatActor : MonoBehaviour
    {
        [SerializeField]
        protected EnemyFinder enemyFinder;

        protected float Cooldown { get; set; } = 2;
        protected float CooldownTimer;

        public virtual bool CanAttack() => CooldownTimer <= 0;

        private void Damage(Entity target, int amount)
        {
            if (!target.IsAlive) return;
            target.TakeDamage(amount);
            CooldownTimer = Cooldown;
        }

        public virtual bool TryAttack(Entity target, int amount)
        {
            if (!CanAttack()) return false;

            Damage(target, amount);
            return true;
        }

        protected virtual void Update()
        {
            if (CooldownTimer > 0) CooldownTimer -= Time.deltaTime;
        }
    }
}
