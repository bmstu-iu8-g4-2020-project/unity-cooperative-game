using System.Collections.Generic;
using Entities.PerTickAttribute;
using Entities.Player.States;
using Gameplay;
using UnityEngine;

namespace Entities.Player
{
    /// <summary>
    ///     Main character script, singleton access
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCombatActor))]
    public class PlayerController : Actor
    {
        #region Variables

        public PlayerMovement PlayerMovement { get; private set; }
        public ItemContainer Inventory { get; private set; } //TODO use polymorphism
        public Equipment Equipment { get; private set; }
        public new PlayerStats Stats { get; private set; }

        public new PlayerCombatActor CombatActor { get; private set; }
        //TODO add Endurance

        public Thirst Thirst { get; private set; }
        public Hunger Hunger { get; private set; }
        public Temperature Temperature { get; private set; }

        #region StateMachine

        public StateMachine StateMachine;
        public WalkState WalkState { get; private set; }
        public StealthState StealthState { get; private set; }
        public SprintingState SprintState { get; private set; }
        public CharacterController Controller { get; private set; }

        #endregion

        public bool IsAiming { get; } = false;

        public static readonly List<PlayerController> AllPlayers = new List<PlayerController>();
        public static PlayerController LocalPlayer { get; private set; } //Singleton access

        #region Private

        #endregion

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            PlayerMovement = GetComponent<PlayerMovement>();
            Inventory = GetComponent<ItemContainer>();
            Equipment = GetComponent<Equipment>();
            Controller = GetComponent<CharacterController>();

            Stats = GetComponent<PlayerStats>();
            CombatActor = GetComponent<PlayerCombatActor>();

            // Thirst = new Thirst(7000, -1000, Stats.ThirstResist);
            // Hunger = new Hunger(10000, -1000, Stats.HungerResist);
            // Temperature = new Temperature(2000, -100, Stats.TemperatureResist);
            Thirst = GetComponent<Thirst>();
            Hunger = GetComponent<Hunger>();
            Temperature = GetComponent<Temperature>();
        }

        private void Update()
        {
            if (!isLocalPlayer) return;

            StateMachine.CurrentState.Tick();
            StateMachine.CurrentState.MachineUpdate();
        }

        protected override void LateUpdate()
        {
            foreach (Animator animator in GetComponentsInChildren<Animator>())
            {
                animator.SetBool("DEAD", IsAlive);
                animator.SetBool("CROUCHING", StateMachine.CurrentState is StealthState);
                animator.SetBool("CLIMBING", StateMachine.CurrentState is ClimbingState);
            }
        }

        #region Delete //TODO delete

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "WalkToTrigger":
                    var destination = other.transform.GetChild(0).position;
                    StateMachine.ChangeState(new WalkToState(this, StateMachine, destination, true));
                    break;
            }
        }

        #endregion

        #endregion

        #region NetworkBehaviour Callbacks

        public override void OnStartLocalPlayer()
        {
            LocalPlayer = this;
            gameObject.name = "LocalPlayer";

            GameManager.Instance.GetCamera().gameObject.GetComponentInParent<CameraManager>().SetTarget(transform);

            #region State Machine Init

            StateMachine = new StateMachine();

            WalkState = new WalkState(this, StateMachine);
            StealthState = new StealthState(this, StateMachine);
            SprintState = new SprintingState(this, StateMachine);

            StateMachine.Initialize(WalkState);

            #endregion
        }

        public override void OnStartServer()
        {
            AllPlayers.Add(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}
