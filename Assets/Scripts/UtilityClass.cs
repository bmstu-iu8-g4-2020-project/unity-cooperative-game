using UnityEngine;
using UnityEngine.AI;

public static class UtilityClass
{
    public static Vector3 GetRandomDir() =>
        new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

    //Random point on NavMesh in circle
    public static Vector3 RandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier)
    {
        Vector2 r = UnityEngine.Random.insideUnitCircle * radiusMultiplier;

        Vector3 randomPosition = new Vector3(position.x + r.x, position.y, position.z + r.y);

        // Raycast to find valid point on NavMesh. otherwise return original one
        return NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, radiusMultiplier * 2, NavMesh.AllAreas)
            ? hit.position
            : position;
    }

    //Random point on NavMesh that has no obstacles (walls) between point and center
    public static Vector3 ReachableRandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier,
        int solverAttempts)
    {
        for (int i = 0; i < solverAttempts; ++i)
        {
            Vector3 candidate = RandomUnitCircleOnNavMesh(position, radiusMultiplier);

            if (!NavMesh.Raycast(position, candidate, out NavMeshHit hit, NavMesh.AllAreas))
                return candidate;
        }

        return position;
    }
}
