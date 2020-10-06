using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [field: SerializeField] public float ViewRadius { get; private set; }

    [field: SerializeField]
    [field: Range(0, 360)]
    public float ViewAngle { get; private set; }

    [SerializeField] private LayerMask entitiesMask;
    [SerializeField] private LayerMask obstacleMask;

    [field: SerializeField] public List<Transform> VisibleTargets { get; } = new List<Transform>();

    [SerializeField] private float meshResolution = 1;
    [SerializeField] private int edgeResolveIterations = 5;
    [SerializeField] private float edgeDistanceThreshold = 0.5f;

    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh _viewMesh;

    private readonly List<Vector3> _viewPoints = new List<Vector3>();
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    [SerializeField] private uint maxZombieCountForDetection = 100;
    private Collider[] _targetsInViewRadius;

    private void Awake()
    {
        _targetsInViewRadius = new Collider[maxZombieCountForDetection];
    }


    private void Start()
    {
        _viewMesh = new Mesh {name = "View Mash"};
        viewMeshFilter.mesh = _viewMesh;

        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        VisibleTargets.Clear();
        var size = Physics.OverlapSphereNonAlloc(transform.position, ViewRadius, _targetsInViewRadius, entitiesMask);

        for (int i = 0; i < size; i++)
        {
            Transform target = _targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) <= ViewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    // target.GetComponent<VisibilityController>().Show();
                    VisibleTargets.Add(target);
                }
            }
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
        float stepAngleSize = ViewAngle / stepCount;
        _viewPoints.Clear();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            Debug.DrawLine(transform.position, newViewCast.Point);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded =
                    Mathf.Abs(oldViewCast.Dist - newViewCast.Dist) > edgeDistanceThreshold;
                if (oldViewCast.Hit != newViewCast.Hit ||
                    (oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        _viewPoints.Add(edge.PointA);
                        Debug.DrawLine(transform.position, edge.PointA);
                    }

                    if (edge.PointB != Vector3.zero)
                    {
                        _viewPoints.Add(edge.PointB);
                        Debug.DrawLine(transform.position, edge.PointB);
                    }
                }
            }

            _viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        int vertexCount = _viewPoints.Count + 1;
        _vertices.Clear();
        _vertices.Add(Vector3.zero); // center of character
        _triangles.Clear();

        for (int i = 0; i < vertexCount - 1; i++)
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

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, ViewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;


        bool edgeDistanceThresholdExceeded =
            Mathf.Abs(minViewCast.Dist - maxViewCast.Dist) > edgeDistanceThreshold;
        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

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
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.rotation.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private struct ViewCastInfo
    {
        public bool Hit;
        public Vector3 Point;
        public float Dist;
        public float Angle;

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
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
