using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;

    private bool isDragging = false;
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
            (Vector2 jumpDirection, float jumpForce) = JumpForceEnd(touch);
            Jump(jumpDirection, jumpForce);
        }
    }

    private void JumpForceStart(Touch touch)
    {
        // Handle touch begin
        if (touch.phase == TouchPhase.Began)
        {
            isDragging = true;

            // Get the touch position
            startTouchPosition = touch.position;
        }
    }

    private (Vector2, float) JumpForceEnd(Touch touch)
    {
        // Handle touch end
        if (touch.phase is TouchPhase.Ended)
        {
            isDragging = false;

            // Get the touch release position
            endTouchPosition = touch.position;

            // Calculate the drag vector
            Vector2 dragVector = startTouchPosition - endTouchPosition;

            // Get the jump force and direction
            return (dragVector.normalized, dragVector.magnitude);
        }

        // If the player didn't release, don't move the ball
        return (Vector2.zero, 0);
    }

    private void Jump(Vector2 jumpDirection, float jumpForce)
    {
        // Unlock the ball movement
        player.constraints = RigidbodyConstraints2D.None;

        // Apply the force to the ball
        player.AddForce(jumpDirection * Math.Clamp(jumpForce, 0, 800), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When touching a platform, stop all movement
        if (collision.collider.tag is "Platform")
        {
            player.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }
}