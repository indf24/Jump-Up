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

    private void Awake()
    {
        CreatePool();
    }

    void Start()
    {
        trajectoryRenderer = GetComponent<LineRenderer>();
    }

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

    public void CalculateTrajectory(Touch touch, Rigidbody2D rigidbody, Vector2 jumpVector)
    {
        if (touch.phase is TouchPhase.Moved or TouchPhase.Stationary)
        {
            trajectoryRenderer.enabled = true;
            List<Vector2> trajectory = Plot(rigidbody, (Vector2)rigidbody.transform.position, jumpVector, 1000);

            trajectoryRenderer.positionCount = trajectory.Count;

            Vector3[] positions = new Vector3[trajectory.Count];

            for (int i = 0; i < trajectory.Count; i++)
            {
                positions[i] = trajectory[i];
            }

            trajectoryRenderer.SetPositions(positions);
        }
        else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
        {
            DespawnDots();
        }
    }

    private List<Vector2> Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 dragVector, int steps)
    {
        DespawnDots();

        List<Vector2> results = new();

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = rigidbody.gravityScale * timestep * timestep * Physics2D.gravity;

        float drag = 1f - (timestep * rigidbody.drag);
        Vector2 moveStep = dragVector / rigidbody.mass * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;

            results.Add(pos);
        }

        int sampleRate = 10; // Sample every 5th point
        List<Vector2> sampledResults = new();

        for (int i = 0; i < results.Count; i += sampleRate)
        {
            TrajectoryDot dot = SpawnDot(results[i]);
            dot.transform.parent = transform;
            sampledResults.Add(results[i]);
        }

        return sampledResults;
    }

    private TrajectoryDot SpawnDot(Vector2 spawnPosition)
    {
        TrajectoryDot dot;

        if (trajectoryDotPool.Any())
        {
            dot = trajectoryDotPool.Dequeue();
            dot.Spawn(spawnPosition);
        }
        else
        {
            GameObject obj = Instantiate(trajectoryDotPrefab, spawnPosition, Quaternion.identity);
            dot = obj.GetComponent<TrajectoryDot>();
        }

        return dot;
    }

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
