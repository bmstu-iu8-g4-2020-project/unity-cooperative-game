﻿using System;
using Entities.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Entities.Zombie
{
    public enum AttractionSource //Sorted by priority
    {
        Player,
        Sound,
        Light
    }

    //TODO add Cone of Sight
    [RequireComponent(typeof(ZombieStats))]
    public class Zombie : Actor
    {
        public new ZombieStats Stats { get; private set; }

        private enum State
        {
            Roaming,
            ChaseTarget
        }

        private State _state;
        private Tuple<Transform, AttractionSource> _target;

        private NavMeshAgent _navMeshAgent;
        private SphereCollider _agroTrigger;

        private void Awake() => _navMeshAgent = GetComponent<NavMeshAgent>();

        protected override void Start()
        {
            base.Start();
            Stats = GetComponent<ZombieStats>();
            _state = State.Roaming;
            _agroTrigger = GetComponent<SphereCollider>();
            _navMeshAgent.speed = Stats.roamingSpeed.GetModified();
            _navMeshAgent.updateRotation = false;
        }

        private float _timer = 0;

        [SerializeField]
        private float attractionTime = 20f;

        private float _attractionSourceTimer = 0;

        [SerializeField]
        private float minStayTime = 10f;

        [SerializeField]
        private float maxStayTime = 30f;

        private float _attackRadius = 1f;

        private void Update()
        {
            if (_attractionSourceTimer > 0) _attractionSourceTimer -= Time.deltaTime;
            if (_target != null && _attractionSourceTimer <= 0) ForgetTarget();

            switch (_state)
            {
                case State.Roaming:
                    if (_navMeshAgent.remainingDistance <= 0.1f)
                    {
                        if (_timer <= 0)
                        {
                            _navMeshAgent.SetDestination(GetRoamingPositionFromNavMesh());
                            _timer = Random.Range(minStayTime, maxStayTime);
                        }

                        _timer -= Time.deltaTime;
                    }

                    if (_target != null)
                    {
                        _state = State.ChaseTarget;
                        _navMeshAgent.speed = Stats.chaseSpeed.GetModified();
                    }

                    break;
                case State.ChaseTarget:
                    if (_target == null)
                    {
                        _state = State.Roaming;
                        _navMeshAgent.speed = Stats.roamingSpeed.GetModified();
                    }
                    else
                    {
                        _navMeshAgent.SetDestination(_target.Item1.position);

                        if (Vector3.Distance(_target.Item1.position, transform.position) <= _attackRadius)
                        {
                            if (_target.Item1.TryGetComponent(out Entity entity))
                            {
                                CombatActor.TryAttack(entity, Stats.attack.GetModified());
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Fix slow rotation of navMeshAgent
            InstantlyTurn(_navMeshAgent.destination);
        }

        public bool TrySetTarget(Transform pTransform, AttractionSource sourceType)
        {
            if (_target != null)
            {
                //Check target's priority
                if ((int) sourceType > (int) _target.Item2)
                {
                    return SetTargetAndStartTimer(pTransform, sourceType);
                }

                if ((int) sourceType == (int) _target.Item2 && sourceType == AttractionSource.Player)
                {
                    if (Vector3.Distance(pTransform.position, transform.position) >=
                        Vector3.Distance(_target.Item1.position, transform.position))
                    {
                        return SetTargetAndStartTimer(pTransform, sourceType);
                    }
                }
            }
            else
            {
                return SetTargetAndStartTimer(pTransform, sourceType);
            }

            return false;
        }

        private bool SetTargetAndStartTimer(Transform pTransform, AttractionSource sourceType)
        {
            SetTarget(pTransform, sourceType);
            _attractionSourceTimer = attractionTime;
            if (pTransform.TryGetComponent(out Entity entity))
            {
                entity.OnDying += ForgetTarget;
            }

            return true;
        }

        private void ForgetTarget()
        {
            if (_target.Item1.TryGetComponent(out Entity entity)) entity.OnDying -= ForgetTarget;
            _target = null;
        }

        private void SetTarget(Transform pTransform, AttractionSource sourceType) =>
            _target = new Tuple<Transform, AttractionSource>(pTransform, sourceType);

        private Vector3 GetRoamingPosition() =>
            transform.position + UtilityClass.GetRandomDir() * Random.Range(1f, 7f);

        private Vector3 GetRoamingPositionFromNavMesh() =>
            UtilityClass.ReachableRandomUnitCircleOnNavMesh(transform.position, Random.Range(1f, 7f), 6);

        private void InstantlyTurn(Vector3 destination)
        {
            if ((destination - transform.position).magnitude < 0.1f) return;

            transform.LookAtXZ(destination);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
                TrySetTarget(player.transform, AttractionSource.Player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_target != null && other.transform == _target.Item1)
                ForgetTarget(); //TODO mb check distance in update for lose target
        }
    }
}