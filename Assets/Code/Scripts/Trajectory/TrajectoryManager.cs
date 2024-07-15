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

    public void MakeTrajectory(Touch touch, Rigidbody2D rigidbody, Vector2 jumpVector, int steps)
    {
        if (touch.phase is TouchPhase.Moved)
        {
            DespawnDots();
            List<Vector2> trajectoryPoints = CalculateTrajectory(rigidbody, (Vector2)rigidbody.transform.position, jumpVector, steps);
            DrawTrajectory(trajectoryPoints);
        }
    }

    private List<Vector2> CalculateTrajectory(Rigidbody2D rigidbody, Vector2 pos, Vector2 dragVector, int steps)
    {
        List<Vector2> points = new();

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAcceleration = rigidbody.gravityScale * timestep * timestep * Physics2D.gravity;

        float drag = 1f - (timestep * rigidbody.drag);
        Vector2 moveStep = dragVector / rigidbody.mass * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAcceleration;
            moveStep *= drag;
            pos += moveStep;

            points.Add(pos);
        }

        return points;
    }

    private void DrawTrajectory(List<Vector2> trajectoryPoints)
    {
        int sampleRate = 10;

        for (int i = 0; i < trajectoryPoints.Count; i += sampleRate)
        {
            TrajectoryDot dot = SpawnDot(trajectoryPoints[i]);
            dot.transform.parent = transform;
        }
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
