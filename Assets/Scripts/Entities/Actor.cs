using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities
{
    //Or Creature
    [RequireComponent(typeof(CombatActor))]
    public class Actor : Entity
    {
        //TODO add movement
        public CombatActor CombatActor { get; private set; }
        public Animator animator { get; private set; }

        protected override void Start()
        {
            base.Start();
            CombatActor = GetComponent<CombatActor>();
        }


        protected virtual void LateUpdate()
        {
            foreach (Animator animator in GetComponentsInChildren<Animator>())
            {
                animator.SetBool("DEAD", IsAlive);
            }
        }
    }
}
