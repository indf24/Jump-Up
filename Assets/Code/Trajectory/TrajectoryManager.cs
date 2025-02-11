using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    internal static TrajectoryManager instance;

    [SerializeField] private GameObject trajectoryBarPrefab;
    [SerializeField] private GameObject trajectoryFilledBarPrefab;

    private const int poolSize = 10;

    private Queue<TrajectoryBar> trajectoryBarPool = new();
    private Queue<TrajectoryBar> trajectoryFilledBarPool = new();
    private const int maxTrajectorySteps = 10;

    private enum BarType { Emtpy, Filled };

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        CreatePool();
    }

    // Creates a pool of trajectory bars to use throughout the game
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            TrajectoryBar bar = Instantiate(trajectoryBarPrefab).GetComponent<TrajectoryBar>();
            bar.Despawn();
            trajectoryBarPool.Enqueue(bar);

            bar = Instantiate(trajectoryFilledBarPrefab).GetComponent<TrajectoryBar>();
            bar.Despawn();
            trajectoryFilledBarPool.Enqueue(bar);
        }
    }

    // Creates a new trajectory when needed
    public void MakeTrajectory(Rigidbody2D rigidbody, Vector2 jumpVector, int minJumpForce, int maxJumpForce)
    {
        float jumpMagnitude = Mathf.Round(jumpVector.magnitude);
        int steps = Mathf.RoundToInt(maxTrajectorySteps * ((jumpMagnitude - minJumpForce) / (maxJumpForce - minJumpForce)));

        DespawnBars();
        List<Vector2> trajectoryPoints = CalculateTrajectory(rigidbody.transform.position, jumpVector);
        DrawTrajectory(trajectoryPoints, steps);
        RotateBars(jumpVector);
    }

    // Calculates where the trajectory bars should be placed
    private List<Vector2> CalculateTrajectory(Vector2 startPos, Vector2 jumpVector)
    {
        List<Vector2> points = new();

        // Fixed trajectory length
        float totalLength = 5f;

        // Calculate the normalized direction
        Vector2 normalizedDirection = jumpVector.normalized;

        // Place bars progressively along the trajectory length
        for (int i = 0; i < maxTrajectorySteps; i++)
        {
            Vector2 point = startPos + (normalizedDirection * (totalLength / maxTrajectorySteps * i));
            points.Add(point);
        }

        return points;
    }

    // Places the trajectory bars
    private void DrawTrajectory(List<Vector2> trajectoryPoints, float steps)
    {
        int sampleRate = 1;

        for (int i = 1; i < maxTrajectorySteps; i++)
        {
            TrajectoryBar bar = SpawnBar(trajectoryPoints[i]);
            bar.transform.parent = transform;
        }

        for (int i = 1; i < steps; i += sampleRate)
        {
            TrajectoryBar bar = SpawnBar(trajectoryPoints[i], BarType.Filled);
            bar.transform.parent = transform;
        }
    }

    private TrajectoryBar GetTrajectoryBar(Queue<TrajectoryBar> pool, GameObject prefab) => pool.Any() ? pool.Dequeue() : Instantiate(prefab).GetComponent<TrajectoryBar>();

    private TrajectoryBar SpawnBar(Vector2 spawnPosition, BarType type = BarType.Emtpy)
    {
        TrajectoryBar bar = type switch
        {
            BarType.Emtpy => GetTrajectoryBar(trajectoryBarPool, trajectoryBarPrefab),
            BarType.Filled => GetTrajectoryBar(trajectoryFilledBarPool, trajectoryFilledBarPrefab),
        };

        bar.Spawn(spawnPosition);
        return bar;
    }

    // Despawns all the trajectory bars
    public void DespawnBars()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            TrajectoryBar bar = transform.GetChild(i).GetComponent<TrajectoryBar>();
            bar.Despawn();

            if (bar.name.Contains("Filled"))
            {
                trajectoryFilledBarPool.Enqueue(bar);
            }
            else
            {
                trajectoryBarPool.Enqueue(bar);
            }
        }
    }

    public void RotateBars(Vector2 direction)
    {
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90; // Convert to degrees
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = Quaternion.Euler(0, 0, angle); // Apply rotation only on the z-axis
        }
    }
}
