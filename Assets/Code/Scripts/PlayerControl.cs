using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D player;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            JumpForceStart(touch);
            JumpForceEnd(touch);
        }
    }

    private void JumpForceStart(Touch touch)
    {
        // Handle touch begin
        if (touch.phase == TouchPhase.Began)
        {
            // Get the touch position
            startTouchPosition = touch.position;
        }
    }

    private void JumpForceEnd(Touch touch)
    {
        // Handle touch end
        if (touch.phase is TouchPhase.Ended)
        {
            // Get the vector direction and force
            endTouchPosition = touch.position;
            Vector2 dragVector = startTouchPosition - endTouchPosition;
            (Vector2 jumpDirection, float jumpForce) = (dragVector.normalized, dragVector.magnitude);
            Debug.Log(jumpDirection);

            if (jumpForce > 0)
            {
                Jump(jumpDirection, jumpForce);
            }
        }
    }

    private void Jump(Vector2 jumpDirection, float jumpForce)
    {
        // Apply the force to the ball
        player.AddForce(jumpDirection * Math.Clamp(jumpForce, 0, 800), ForceMode2D.Impulse);
    }
}