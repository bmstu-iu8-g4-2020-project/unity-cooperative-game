using Data.Items;
using Entities.Player;
using Entities.Player.States;
using UnityEngine;

namespace Entities
{
    public class PlayerCombatActor : CombatActor
    {
        private PlayerController _player;

        [SerializeField]
        private float pushDistance = 0.3f;

        public override bool CanAttack() => base.CanAttack() && _player.StateMachine.CurrentState is StealthState;

        private void Awake() => _player = GetComponent<PlayerController>();

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

        protected override void Update()
        {
            base.Update();

            if (PlayerControls.Instance.IsPressPush())
            {
                Push(enemyFinder.GetNearest(_maxZombieForPush));
            }
            else if (PlayerControls.Instance.IsPressAttack() && CanAttack())
            {
                if (!_player.Equipment.TryGetItemInMainHand(out _handItem)) return;

                CooldownTimer = _handItem.CooldownTime;
                foreach (var entity in enemyFinder.GetNearest(_handItem.TargetsPerHit))
                {
                    TryAttack(entity, _player.Stats.Attack.GetModified());
                }
            }
        }
    }
}
