using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlatformManager platformManager;

    private Rigidbody2D player;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    Vector2 jumpVector;

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
            jumpVector = JumpCalculations(touch);
            CalculateTrajectory(touch);

            if (jumpVector.magnitude > 0 && !isTouching)
            {
                Jump(jumpVector);
                playerManager.canJump = false;
                StartCoroutine(platformManager.DespawnPlatform());
            }
        }
    }

    private Vector2 JumpCalculations(Touch touch)
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

            return jumpVector.normalized;
        }

        return Vector2.zero;
    }

    private void Jump(Vector2 jumpDirection)
    {
        float minJumpForce = 0f;
        float maxJumpForce = 800f;
        // Apply the force to the ball
        //player.AddForce(jumpDirection * 800f, ForceMode2D.Impulse);
    }

    private void CalculateTrajectory(Touch touch)
    {
        if (touch.phase is TouchPhase.Moved or TouchPhase.Stationary)
        {
            trajectoryRenderer.enabled = true;

            //Vector2[] trajectory = Plot(player, (Vector2)transform.position, jumpVector.normalized * 800f, 1000);

            trajectoryRenderer.positionCount = trajectory.Length;

            Vector3[] positions = new Vector3[trajectory.Length];

            for (int i = 0; i < trajectory.Length; i++)
            {
                positions[i] = trajectory[i];
            }

            trajectoryRenderer.SetPositions(positions);
        }
        else
        {
            //trajectoryRenderer.enabled = false;
        }
    }

    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 dragVector, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = rigidbody.gravityScale * timestep * timestep * Physics2D.gravity;

        float drag = 1f - (timestep * rigidbody.drag);
        Vector2 moveStep = dragVector * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }
}