using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    //TODO disable on client for zombie
    public class FieldOfView : MonoBehaviour
    {
        [Header("Enemy detection")]
        [SerializeField]
        protected LayerMask entitiesMask;

        [SerializeField]
        protected LayerMask obstacleMask;

        [SerializeField]
        protected uint maxTargetsCountForDetection = 100;

        protected Collider[] TargetsInViewRadius;

        protected readonly float TargetsScanDelay = 0.2f;

        [field: SerializeField]
        public float ViewRadius { get; set; } = 20f;

        [field: SerializeField]
        [field: Range(0, 360)]
        public float ViewAngle { get; set; } = 180;

        public List<Transform> VisibleTargets { get; } = new List<Transform>();

        protected virtual void Awake() => TargetsInViewRadius = new Collider[maxTargetsCountForDetection];

        protected virtual void OnEnable() =>
            InvokeRepeating(nameof(FindVisibleTargets), TargetsScanDelay, TargetsScanDelay);

        private void OnDisable() => CancelInvoke(nameof(FindVisibleTargets));

        private IEnumerator FindTargetWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void FindVisibleTargets()
        {
            VisibleTargets.Clear();
            //TODO mb change to OnTriggerEnter\Exit
            var size = Physics.OverlapSphereNonAlloc(transform.position, ViewRadius, TargetsInViewRadius,
                entitiesMask);

            for (var i = 0; i < size; i++)
            {
                var target = TargetsInViewRadius[i].transform;
                var dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) <= ViewAngle / 2)
                {
                    var distToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        OnEnemyDetection?.Invoke(target);
                        VisibleTargets.Add(target);
                    }
                }
            }
        }

        public event Action<Transform> OnEnemyDetection;

        public bool InAngleOfView(Vector3 dirToTarget) =>
            Vector3.Angle(transform.forward, dirToTarget) <= ViewAngle / 2;

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal) angleInDegrees += transform.rotation.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
