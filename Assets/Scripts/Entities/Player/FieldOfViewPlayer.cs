using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Entities.Player
{
    /// <summary>
    /// Field of view with visualisation
    /// </summary>
    public class FieldOfViewPlayer : FieldOfView
    {
        [Header("Visualization")]
        [SerializeField]
        private float meshResolution = 1;

        [SerializeField]
        private int edgeResolveIterations = 5;

        [SerializeField]
        private float edgeDistanceThreshold = 0.5f;

        [SerializeField]
        private MeshFilter viewMeshFilter;

        [SerializeField]
        private bool drawDebugLines;

        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector3> _viewPoints = new List<Vector3>();
        private Mesh _viewMesh;

        private void Start()
        {
            _viewMesh = new Mesh {name = "View Mesh"};
            viewMeshFilter.mesh = _viewMesh;

            OnEnemyDetection += target => target.GetComponent<VisibilityController>()?.Show();
        }

        private void LateUpdate() => DrawFieldOfView();

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
