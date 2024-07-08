using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlatformManager platformManager;
    [SerializeField] private TrajectoryManager trajectoryManager;

    private Rigidbody2D player;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    float minJumpForce = 0f;
    float maxJumpForce = 800f;

    Vector2 currentJumpVector;

    private bool isTouching;

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
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
                trajectoryManager.CalculateTrajectory(touch, player, jumpVector);
                currentJumpVector = jumpVector;
            }

            if (jumpVector.magnitude > 0 && !isTouching)
            {
                Jump(jumpVector, minJumpForce, maxJumpForce);
                playerManager.canJump = false;
                trajectoryManager.DespawnDots();
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

}