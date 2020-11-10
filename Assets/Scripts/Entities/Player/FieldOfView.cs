using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Entities.Player
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField]
        private LayerMask entitiesMask;

        [SerializeField]
        private LayerMask obstacleMask;

        [SerializeField]
        private float meshResolution = 1;

        [SerializeField]
        private int edgeResolveIterations = 5;

        [SerializeField]
        private float edgeDistanceThreshold = 0.5f;

        [SerializeField]
        private MeshFilter viewMeshFilter;

        [SerializeField]
        private uint maxZombieCountForDetection = 100;

        [SerializeField]
        private bool drawDebugLines;

        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();

        private readonly List<Vector3> _viewPoints = new List<Vector3>();

        private Collider[] _targetsInViewRadius;

        private Mesh _viewMesh;

        private readonly float targetsScanDelay = 0.2f;

        [field: SerializeField]
        public float ViewRadius { get; set; } = 20f;

        [field: SerializeField]
        [field: Range(0, 360)]
        public float ViewAngle { get; set; }

        [field: SerializeField]
        public List<Transform> VisibleTargets { get; } = new List<Transform>();

        [SerializeField]
        private void Awake()
        {
            _targetsInViewRadius = new Collider[maxZombieCountForDetection];
        }


        private void Start()
        {
            _viewMesh = new Mesh {name = "View Mesh"};
            viewMeshFilter.mesh = _viewMesh;

            StartCoroutine(FindTargetWithDelay(targetsScanDelay));
        }

        private void LateUpdate() => DrawFieldOfView();

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
            var size = Physics.OverlapSphereNonAlloc(transform.position, ViewRadius, _targetsInViewRadius,
                entitiesMask);

            for (var i = 0; i < size; i++)
            {
                var target = _targetsInViewRadius[i].transform;
                var dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) <= ViewAngle / 2)
                {
                    var distToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        target.GetComponent<VisibilityController>()?.Show(); //Show Enemy
                        VisibleTargets.Add(target);
                    }
                }
            }
        }

        private void DrawFieldOfView()
        {
            var stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
            var stepAngleSize = ViewAngle / stepCount;
            _viewPoints.Clear();
            var oldViewCast = new ViewCastInfo();

            for (var i = 0; i <= stepCount; i++)
            {
                var angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                var newViewCast = ViewCast(angle);
                if (drawDebugLines) Debug.DrawLine(transform.position, newViewCast.Point);

                if (i > 0)
                {
                    var edgeDistanceThresholdExceeded =
                        Mathf.Abs(oldViewCast.Dist - newViewCast.Dist) > edgeDistanceThreshold;
                    if (oldViewCast.Hit != newViewCast.Hit ||
                        oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded)
                    {
                        var edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.PointA != Vector3.zero)
                        {
                            _viewPoints.Add(edge.PointA);
                            if (drawDebugLines) Debug.DrawLine(transform.position, edge.PointA);
                        }

                        if (edge.PointB != Vector3.zero)
                        {
                            _viewPoints.Add(edge.PointB);
                            if (drawDebugLines) Debug.DrawLine(transform.position, edge.PointB);
                        }
                    }
                }

                _viewPoints.Add(newViewCast.Point);
                oldViewCast = newViewCast;
            }

            var vertexCount = _viewPoints.Count + 1;
            _vertices.Clear();
            _vertices.Add(Vector3.zero); // center of character
            _triangles.Clear();

            for (var i = 0; i < vertexCount - 1; i++)
            {
                _vertices.Add(transform.InverseTransformPoint(_viewPoints[i]));
                if (i < vertexCount - 2)
                {
                    _triangles.Add(0);
                    _triangles.Add(i + 1);
                    _triangles.Add(i + 2);
                }
            }

            _viewMesh.Clear();
            _viewMesh.vertices = _vertices.ToArray();
            _viewMesh.triangles = _triangles.ToArray();
            _viewMesh.RecalculateNormals();
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            var dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, ViewRadius, obstacleMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            var minAngle = minViewCast.Angle;
            var maxAngle = maxViewCast.Angle;
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;


            var edgeDistanceThresholdExceeded =
                Mathf.Abs(minViewCast.Dist - maxViewCast.Dist) > edgeDistanceThreshold;
            for (var i = 0; i < edgeResolveIterations; i++)
            {
                var angle = (minAngle + maxAngle) / 2;
                var newViewCast = ViewCast(angle);

                if (newViewCast.Hit == minViewCast.Hit && !edgeDistanceThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal) angleInDegrees += transform.rotation.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private struct ViewCastInfo
        {
            public readonly bool Hit;
            public readonly Vector3 Point;
            public readonly float Dist;
            public readonly float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float dist, float angle)
            {
                Hit = hit;
                Point = point;
                Dist = dist;
                Angle = angle;
            }
        }

        private struct EdgeInfo
        {
            public readonly Vector3 PointA;
            public readonly Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
    }
}
