using System;
using Entities.Player;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Entities.Zombie
{
    public enum AttractionSourceType //Sorted by priority
    {
        Player,
        Sound,
        Light
    }

    //TODO mb use NavMesh Off-mesh Link for windows and doors(at least for turning on / off it when opening / closing the door)
    //TODO or add triggers for doors and windows, that will say to zombie "stop and attack me until you destroy",
    //after that zombie should enter to house and he can find player again. Also use Navigation Areas and Costs for doorway
    //TODO add Cone of Sight
    [RequireComponent(typeof(ZombieStats))]
    [RequireComponent(typeof(FieldOfView))]
    public class Zombie : Actor
    {
        public new ZombieStats Stats { get; private set; }

        private FieldOfView _fieldOfView;

        public enum State
        {
            Roaming,
            ChaseTarget,
            Attack,
            DestroyObstacle
        }

        private State _state;

        private Tuple<Transform, AttractionSourceType> _target;

        private Vector3 _lastSeenDirection;
        private NavMeshAgent _navMeshAgent;

        protected override void Start()
        {
            base.Start();
            Stats = GetComponent<ZombieStats>();
            _fieldOfView = GetComponent<FieldOfView>();
            _fieldOfView.enabled = isServer;
            if (isServer)
            {
                _fieldOfView.OnEnemyDetection += target => TrySetTarget(target, AttractionSourceType.Player);
            }

            _state = State.Roaming;

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.enabled = isServer;
            _navMeshAgent.speed = Stats.roamingSpeed.GetModified();
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.stoppingDistance = attackRadius;
        }

        private float _timer = 0;

        [SerializeField]
        private float attractionTime = 5f; //TODO mb add different time for different attraction source

        private float _attractionSourceTimer = 0;

        [SerializeField]
        private float minStayTime = 10f;

        [SerializeField]
        private float maxStayTime = 30f;

        [SerializeField]
        private float attackRadius = 0.7f;


        //TODO or iterate by VisibleTargets per some time and invoke TrySetTarget for each
        [ServerCallback]
        private void Update()
        {
            if (!isServer) return;

            if (_attractionSourceTimer > 0) _attractionSourceTimer -= Time.deltaTime;
            if (_state != State.DestroyObstacle && _target != null && _attractionSourceTimer <= 0) ForgetTarget();

            switch (_state)
            {
                case State.Roaming:
                    if (_target != null)
                    {
                        _state = State.ChaseTarget;
                        _navMeshAgent.speed = Stats.chaseSpeed.GetModified();
                        _navMeshAgent.SetDestination(_target.Item1.position);
                    }
                    else if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + 0.1f)
                    {
                        if (_timer <= 0)
                        {
                            _navMeshAgent.SetDestination(GetRoamingPositionFromNavMesh());
                            _timer = Random.Range(minStayTime, maxStayTime);
                        }

                        _timer -= Time.deltaTime;
                    }

                    break;
                case State.ChaseTarget:
                    if (_target == null)
                    {
                        _state = State.Roaming;
                        _navMeshAgent.speed = Stats.roamingSpeed.GetModified();
                        //Move to last seen player direction for attractionTime second
                        //In a roaming state, the zombie will still move to this point
                        _navMeshAgent.SetDestination(transform.position +
                                                     //Calculate end point by direction
                                                     _lastSeenDirection * (_navMeshAgent.speed * attractionTime));
                    }
                    else
                    {
                        _lastSeenDirection = _target.Item1.position - transform.position;
                        _navMeshAgent.SetDestination(_target.Item1.position);

                        if (Vector3.Distance(_target.Item1.position, transform.position) <= attackRadius)
                        {
                            _state = State.Attack;
                            CombatActor.StartCooldown(); //For attack with delay
                        }
                    }

                    break;
                case State.DestroyObstacle:
                    if (_target != null && _target.Item1.TryGetComponent(out Entity entity1))
                        if (entity1.IsAlive)
                            CombatActor.TryAttack(entity1, Stats.attack.GetModified());
                        else _state = State.Roaming;
                    break;
                case State.Attack:
                    if (_target == null)
                    {
                        _state = State.Roaming;
                        break;
                    }

                    if (Vector3.Distance(_target.Item1.position, transform.position) >= attackRadius)
                    {
                        _state = State.ChaseTarget;
                        break;
                    }

                    if (_target.Item1.TryGetComponent(out Entity entity))
                        CombatActor.TryAttack(entity, Stats.attack.GetModified());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Fix slow rotation of navMeshAgent
            InstantlyTurn(_navMeshAgent.destination);
        }

        [Command(ignoreAuthority = true)]
        public void CmdTrySetTarget(NetworkIdentity target, int sourceType) =>
            TrySetTarget(target.transform, (AttractionSourceType) sourceType);

        [ServerCallback]
        public bool TrySetTarget(Transform pTransform, AttractionSourceType sourceType)
        {
            if (_target != null)
            {
                if (_target.Item1 == pTransform) return false;
                //Check target's priority
                if ((int) sourceType > (int) _target.Item2)
                {
                    SetTargetAndStartTimer(pTransform, sourceType);
                    return true;
                }

                if (sourceType == _target.Item2 && sourceType == AttractionSourceType.Player)
                {
                    if (Vector3.Distance(pTransform.position, transform.position) <=
                        Vector3.Distance(_target.Item1.position, transform.position))
                    {
                        SetTargetAndStartTimer(pTransform, sourceType);
                        return true;
                    }
                }
            }
            else
            {
                SetTargetAndStartTimer(pTransform, sourceType);
                return true;
            }

            return false;
        }

        [ServerCallback]
        public void SetTargetAndStartTimer(Transform pTransform, AttractionSourceType sourceType)
        {
            SetTarget(pTransform, sourceType);
            _attractionSourceTimer = attractionTime;
            if (pTransform.TryGetComponent(out Entity entity))
            {
                entity.OnDying += ForgetTarget;
            }
        }

        [ServerCallback]
        private void SetTarget(Transform pTransform, AttractionSourceType sourceType) =>
            _target = new Tuple<Transform, AttractionSourceType>(pTransform, sourceType);

        private void ForgetTarget(Entity obj) => ForgetTarget();

        [ServerCallback]
        private void ForgetTarget()
        {
            if (_target.Item1.TryGetComponent(out Entity entity)) entity.OnDying -= ForgetTarget;
            _target = null;
        }

        [ServerCallback]
        public void AttackObstacle(Transform obstacle)
        {
            SetTargetAndStartTimer(obstacle, AttractionSourceType.Player); //TODO add obstacle type mb
            _navMeshAgent.SetDestination(_target.Item1.position);
            _state = State.DestroyObstacle;
        }

        [ServerCallback]
        public void SetRoamingDestination(Vector3 position)
        {
            _state = State.Roaming;
            _navMeshAgent.SetDestination(position);
        }

        private Vector3 GetRoamingPosition() =>
            transform.position + UtilityClass.GetRandomDir() * Random.Range(1f, 7f);

        private Vector3 GetRoamingPositionFromNavMesh() =>
            UtilityClass.ReachableRandomUnitCircleOnNavMesh(transform.position, Random.Range(1f, 7f), 6);

        private void InstantlyTurn(Vector3 destination)
        {
            if ((destination - transform.position).magnitude < 0.1f) return;
            transform.LookAtXZ(destination);
        }
    }
}
