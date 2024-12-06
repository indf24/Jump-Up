using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    private static bool playerInputAllowed = false;
    public static bool PlayerInputAllowed => playerInputAllowed;

    private RaycastHit2D hit;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        EventHub.OnPlatformCollision += PlatformCollision;
    }

    private void OnDisable()
    {
        EventHub.OnPlatformCollision -= PlatformCollision;
    }

    private void OnDestroy()
    {
        EventHub.OnPlatformCollision -= PlatformCollision;
    }

    // Methods to safely modify the value
    public static void EnableInput() => playerInputAllowed = true;
    public static void DisableInput() => playerInputAllowed = false;

    // Stops the player
    private void StopMovement()
    { 
        playerRb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        playerRb.velocity = Vector2.zero;
    }

    // Checks if the player is in contact the top of a platform  
    private bool IsGrounded()
    {
        return hit.collider != null && hit.collider.CompareTag("Platform");
    }

    // Creates a raycast for detection of contact between the player and a platform
    private void CreateGroudedRay()
    {
        Vector2 rayOrigin = playerRb.position - new Vector2(0, (playerCollider.bounds.size.y / 2) + 0.002f);
        float rayDistance = 0.1f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);

        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, Color.red, 0.1f);
    }

    // Executes a sequence of events if the player is on top of a platform
    public void PlatformCollision()
    {
        CreateGroudedRay();
        if (IsGrounded())
        {
            StopMovement();
            Debug.Log("aaa");
            EventHub.PlayerLand(1);
        }
    }
}
