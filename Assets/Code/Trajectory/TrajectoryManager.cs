using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    public LineRenderer trajectoryRenderer;

    [SerializeField] private GameObject trajectoryDotPrefab;
    private int poolSize = 100;
    private Queue<TrajectoryDot> trajectoryDotPool = new();
    private int maxTrajectorySteps = 6;

    private void Start()
    {
        CreatePool();
    }

    private void OnEnable()
    {
        EventHub.OnPlayerJump += DespawnDots;
    }

    private void OnDisable()
    {
        EventHub.OnPlayerJump -= DespawnDots;
    }

    private void OnDestroy()
    {
        EventHub.OnPlayerJump -= DespawnDots;
    }

    // Creates a pool of trajectory dots to use throughout the game
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(trajectoryDotPrefab);
            TrajectoryDot dot = obj.GetComponent<TrajectoryDot>();
            dot.Despawn();
            trajectoryDotPool.Enqueue(dot);
        }
    }

    // Creates a new trajectory when needed
    public void MakeTrajectory(Rigidbody2D rigidbody, Vector2 jumpVector, int minJumpForce, int maxJumpForce)
    {
        float steps = maxTrajectorySteps * ((jumpVector.y - minJumpForce) / (maxJumpForce - minJumpForce));

        DespawnDots();
        List<Vector2> trajectoryPoints = CalculateTrajectory(rigidbody.transform.position, jumpVector);
        DrawTrajectory(trajectoryPoints, steps);
    }

    // Calculates where the trajectory dots should be placed
    private List<Vector2> CalculateTrajectory(Vector2 startPos, Vector2 jumpVector)
    {
        List<Vector2> points = new();

        // Fixed trajectory length
        float totalLength = 5f;

        // Calculate the normalized direction
        Vector2 normalizedDirection = jumpVector.normalized;

        // Place dots progressively along the trajectory length
        for (int i = 0; i < maxTrajectorySteps; i++)
        {
            Vector2 point = startPos + normalizedDirection * (totalLength / maxTrajectorySteps * i);
            points.Add(point);
        }

        return points;
    }


    // Places the trajectory dots
    private void DrawTrajectory(List<Vector2> trajectoryPoints, float steps)
    {
        int sampleRate = 1;

        for (int i = 0; i < steps; i += sampleRate)
        {
            TrajectoryDot dot = SpawnDot(trajectoryPoints[i]);
            dot.transform.parent = transform;
        }
    }

    // Spawns a trajectory dot from the pool if any are available, otherwise instantiate a new one and spawn that one
    private TrajectoryDot SpawnDot(Vector2 spawnPosition)
    {
        TrajectoryDot dot;

        if (trajectoryDotPool.Any())
        {
            dot = trajectoryDotPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(trajectoryDotPrefab);
            dot = obj.GetComponent<TrajectoryDot>();
        }

        dot.Spawn(spawnPosition);
        return dot;
    }

    // Despawns all the trajectory dots
    public void DespawnDots()
    {  
        for (int i = 0; i < transform.childCount; i++)
        {
            TrajectoryDot dot = transform.GetChild(i).GetComponent<TrajectoryDot>();
            dot.Despawn();
            trajectoryDotPool.Enqueue(dot);
        }
    }
}
