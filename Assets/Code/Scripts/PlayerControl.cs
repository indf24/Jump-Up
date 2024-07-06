using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlatformManager platformManager;

    private Rigidbody2D player;
    [SerializeField] private GameObject trajectoryDot;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    float minJumpForce = 0f;
    float maxJumpForce = 800f;

    Vector2 currentJumpVector;

    private bool isTouching;

    private LineRenderer trajectoryRenderer;

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
        trajectoryRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && playerManager.canJump)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 jumpVector = CalculateJump(touch);
            if (jumpVector != currentJumpVector)
            {
                jumpVector = LimitJumpForce(jumpVector, minJumpForce, maxJumpForce);
                CalculateTrajectory(touch, jumpVector);
                currentJumpVector = jumpVector;
            }

            if (jumpVector.magnitude > 0 && !isTouching)
            {
                Jump(jumpVector, minJumpForce, maxJumpForce);
                playerManager.canJump = false;
                StartCoroutine(platformManager.DespawnPlatform());
            }
        }
    }

    private Vector2 CalculateJump(Touch touch)
    {
        // Handle touch begin
        if (touch.phase is TouchPhase.Began)
        {
            isTouching = true;
            // Get the touch position
            startTouchPosition = touch.position;
        }

        if (isTouching)
        {
            // Get the vector direction and force  
            endTouchPosition = touch.position;
            Vector2 jumpVector = startTouchPosition - endTouchPosition;

            if (touch.phase is TouchPhase.Ended)
            {
                isTouching = false;
            }

            return jumpVector;
        }

        return Vector2.zero;
    }

    private Vector2 LimitJumpForce(Vector2 jumpVector, float minJumpForce, float maxJumpForce)
    {
        return jumpVector.normalized * Mathf.Clamp(jumpVector.magnitude, minJumpForce, maxJumpForce);
    }

    private void Jump(Vector2 jumpVector, float minJumpForce, float maxJumpForce)
    {
        // Apply the force to the ball
        player.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    private void CalculateTrajectory(Touch touch, Vector2 jumpVector)
    {
        if (touch.phase is TouchPhase.Moved or TouchPhase.Stationary)
        {
            trajectoryRenderer.enabled = true;
            List<Vector2> trajectory = Plot(player, (Vector2)transform.position, jumpVector, 1000);

            trajectoryRenderer.positionCount = trajectory.Count;

            Vector3[] positions = new Vector3[trajectory.Count];

            for (int i = 0; i < trajectory.Count; i++)
            {
                positions[i] = trajectory[i];
            }

            trajectoryRenderer.SetPositions(positions);
        }
        else
        {
            trajectoryRenderer.enabled = false;
        }
    }

    public List<Vector2> Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 dragVector, int steps)
    {
        GameObject trajectory = GameObject.Find("Trajectory");

        for(int i = 0; i < trajectory.transform.childCount; i++)
        {
            Destroy(trajectory.transform.GetChild(i).gameObject);
        }

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

        int sampleRate = 10; // Sample every 5th point (adjust as needed)
        List<Vector2> sampledResults = new();

        for (int i = 0; i < results.Count; i += sampleRate)
        {
            GameObject dot = Instantiate(trajectoryDot, results[i], Quaternion.identity);
            dot.transform.parent = GameObject.Find("Trajectory").transform;
            sampledResults.Add(results[i]);
        }

        return sampledResults;
    }
}