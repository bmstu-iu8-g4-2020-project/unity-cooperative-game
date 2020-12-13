using System.Collections;
using Data.Items;
using Entities.Player;
using Entities.Player.States;
using UnityEngine;

namespace Entities
{
    public class PlayerCombatActor : CombatActor
    {
        [SerializeField]
        private EnemyFinder enemyFinderPush;

        protected IEnemyFinder _enemyFinderPush;

        private PlayerController _player;

        [SerializeField]
        private float pushDistance = 0.3f;

        [SerializeField]
        private float pushCooldown = 1f;

        private float _pushCooldownTimer;

        public override bool CanAttack() => base.CanAttack() && _player.StateMachine.CurrentState is StealthState;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _enemyFinderPush = enemyFinderPush;
        }

        private HandItem _handItem;

        public int debugZombieInRange;

        [SerializeField]
        private uint maxZombieForPush = 3;

        private bool CanPush() => _pushCooldownTimer <= 0;

        private void TryPush(Entity[] entities)
        {
            if (!CanPush()) return;

            foreach (var entity in entities)
            {
                if (!entity.TryGetComponent(out Rigidbody rb)) continue;

                var dir = (entity.transform.position - transform.position).normalized;
                rb.AddForce(dir * pushDistance);
            }

            _pushCooldownTimer = pushCooldown;
        }

        public override bool TryAttack(Entity target, int amount)
        {
            var res = base.TryAttack(target, amount);
            StartCoroutine(AttackAnim(_enemyFinder.GameObject.GetComponent<MeshRenderer>()));
            return res;
        }

        IEnumerator AttackAnim(Renderer renderer)
        {
            var material = renderer.material;

            for (float i = Cooldown; i >= 0; i -= Time.deltaTime)
            {
                var col = material.color;
                col.a = i;
                material.color = col;
                yield return null;
            }
        }

        protected override void Update()
        {
            base.Update();
            debugZombieInRange = _enemyFinderPush.EnemyInRange();

            if (_pushCooldownTimer >= 0) _pushCooldownTimer -= Time.deltaTime;

            if (PlayerControls.Instance.IsPressPush())
            {
                TryPush(_enemyFinderPush.GetNearest(maxZombieForPush));
            }
            else if (PlayerControls.Instance.IsPressAttack() && CanAttack())
            {
                if (!_player.Equipment.TryGetItemInMainHand(out _handItem)) return;

                Cooldown = _handItem.CooldownTime;
                foreach (var entity in _enemyFinder.GetNearest(_handItem.TargetsPerHit))
                {
                    TryAttack(entity, _player.Stats.Attack.GetModified());
                }
            }
        }
    }
}
