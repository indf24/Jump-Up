using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlatformManager platformManager;
    [SerializeField] private TrajectoryManager trajectoryManager;

    private Rigidbody2D player;

    Touch touch = new();
    Vector2 jumpVector = new();

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float minJumpForce = 100f;
    private float maxJumpForce = 800f;

    private int trajectorySteps = 1000;

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerManager.isTouchAllowed)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                jumpVector = CalculateJump(touch);
            }

            if (jumpVector.magnitude > minJumpForce)
            {
                if (touch.phase is TouchPhase.Moved)
                {
                    jumpVector = LimitJumpForce(jumpVector, minJumpForce, maxJumpForce);
                    trajectoryManager.MakeTrajectory(touch, player, jumpVector, trajectorySteps);
                }

                if (touch.phase is TouchPhase.Ended)
                {
                    trajectoryManager.DespawnDots();
                    jumpVector = LimitJumpForce(jumpVector, minJumpForce, maxJumpForce);
                    Jump(jumpVector, minJumpForce, maxJumpForce);
                    playerManager.isTouchAllowed = false;
                    StartCoroutine(playerManager.NotGrounded());
                    StartCoroutine(platformManager.DespawnPlatform());
                    jumpVector = new();
                }
            } 
        }
    }

    private Vector2 CalculateJump(Touch touch)
    {
        // Handle touch begin
        if (touch.phase is TouchPhase.Began)
        {
            // Get the touch position
            startTouchPosition = touch.position;
        }

        if (touch.phase is TouchPhase.Moved or TouchPhase.Ended)
        {
            // Get the vector direction and force  
            endTouchPosition = touch.position;
            Vector2 jumpVector = startTouchPosition - endTouchPosition;

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