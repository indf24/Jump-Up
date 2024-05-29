using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D player;

    private bool isDragging = false;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] private bool platformContact;
    [SerializeField] private bool platformTriggerContact;

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

        if (platformContact && platformTriggerContact && player.velocity != Vector2.zero)
        {
            StopMovement();
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

    private void JumpForceEnd(Touch touch)
    {
        // Handle touch end
        if (touch.phase is TouchPhase.Ended)
        {
            // Get the vector direction and force
            isDragging = false;
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

    private void StopMovement()
    {
        // Stop the ball
        player.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.velocity = Vector2.zero;
        player.constraints = RigidbodyConstraints2D.None;
        platformContact = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag is "Platform")
        {
            platformContact = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag is "Platform")
        {
            platformContact = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag is "Platform")
        {
            platformTriggerContact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag is "Platform")
        {
            platformTriggerContact = false;
        }
    }
}