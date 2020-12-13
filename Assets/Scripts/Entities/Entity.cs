using System;
using Entities.PerTickAttribute;
using Mirror;
using UnityEngine;

namespace Entities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EntityStats))]
    //TODO rework. This class must represent destructible\(mb interactable) entity, not necessary living creature
    public class Entity : NetworkBehaviour
    {
        protected Health Health { get; set; } //Most important part of Entity. Mb make Health not component
        public EntityStats Stats { get; private set; } //TODO rework this class, make attribute's dynamic container

        public bool IsAlive => Health.Current > 0;

        protected virtual void Start()
        {
            Stats = GetComponent<EntityStats>();
            Health = GetComponent<Health>();

            Health.OnEmpty += Die;
        }

        private void OnDestroy() => Health.OnEmpty -= Die;

        public event Action<Entity> OnDying;

        public virtual void Die()
        {
            OnDying?.Invoke(this);
            CmdDie();
            Destroy(gameObject); //For zombie TODO delete
        }

        [Command]
        private void CmdDie() => NetworkServer.Destroy(gameObject);

        public event Action<int> OnTakeDamage; //Subscribe to this event animation's or sound's triggers

        [Command(ignoreAuthority = true)]
        public void CmdTakeDamage(int amount) => TakeDamage(amount);

        [ServerCallback]
        public void TakeDamage(int amount)
        {
            var amountWithResist = amount - Stats.Defence.GetModified();
            Health.Current -= amountWithResist;
            RpcOnTakeDamage(amountWithResist);
            Debug.Log($"Damage {name}: -{amountWithResist}hp");
            //Or add some effects here
        }

        [ClientRpc]
        //TODO mb invoke effects and animation here?
        public void RpcOnTakeDamage(int amount) => OnTakeDamage?.Invoke(amount);

        public event Action<int> OnHeal;

        [Command(ignoreAuthority = true)]
        public void Heal(int amount)
        {
            Health.Current += amount;
            RpcOnHeal(amount);
        }

        [ClientRpc]
        public void RpcOnHeal(int amount) => OnHeal?.Invoke(amount);
    }
}
