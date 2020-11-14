using Data.Items;
using Entities.Player;
using Entities.Player.States;
using UnityEngine;

namespace Entities
{
    public class PlayerCombatActor : MonoBehaviour
    {
        private PlayerController _player;

        [SerializeField]
        private EnemyFinder enemyFinder;

        [SerializeField]
        private float pushDistance = 0.3f;

        private bool CanAttack() => _player.StateMachine.CurrentState is StealthState;

        public void Damage(Entity target, int amount) //TODO mb make it command
        {
            if (!target.IsAlive) return;
            target.TakeDamage(amount);
        }

        private void Awake() => _player = GetComponent<PlayerController>();

        private float _cooldownTimer = 0;
        private HandItem _handItem;

        private uint _maxZombieForPush = 3;

        private void Push(Entity[] entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.TryGetComponent(out CharacterController controller)) continue;
                
                var dir = (entity.transform.position - transform.position).normalized;
                controller.Move(dir * pushDistance);
            }
        }

        private void Update()
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            if (PlayerControls.Instance.IsPressPush())
            {
                Push(enemyFinder.GetNearest(_maxZombieForPush));
            }
            else if (PlayerControls.Instance.IsPressAttack() && CanAttack())
            {
                if (!_player.Equipment.TryGetItemInMainHand(out _handItem)) return;

                _cooldownTimer = _handItem.CooldownTime;
                foreach (var entity in enemyFinder.GetNearest(_handItem.TargetsPerHit))
                {
                    Damage(entity, _player.Stats.Attack.GetModified());
                }
            }
        }
    }
}
