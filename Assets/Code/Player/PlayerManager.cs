using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Collider2D playerCollider;
    private Transform sprite;
    private Animator animator;

    private static bool playerInputAllowed = false;
    public static bool PlayerInputAllowed => playerInputAllowed;

    private RaycastHit2D hit;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<CircleCollider2D>();
        sprite = player.transform.GetChild(1);
        animator = sprite.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHub.OnPlatformCollision += StartPlatformCollision;
        EventHub.RunPlayerAnimation += PlayerAnimation;
    }

    private void OnDisable()
    {
        EventHub.OnPlatformCollision -= StartPlatformCollision;
        EventHub.RunPlayerAnimation -= PlayerAnimation;
    }

    private void OnDestroy()
    {
        EventHub.OnPlatformCollision -= StartPlatformCollision;
        EventHub.RunPlayerAnimation -= PlayerAnimation;
    }

    private void Update()
    {
        sprite.rotation = Quaternion.identity;
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

    private void StartPlatformCollision()
    {
        StartCoroutine(PlatformCollision());
    }

    private IEnumerator PlatformCollision()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            PlayerAnimation("Flying", false);

            CreateGroudedRay();

            if (IsGrounded())
            {
                StopMovement();
                EventHub.PlayerLand(1);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void PlayerAnimation(string animation, bool state)
    {
        animator.SetBool(animation, state);
    }
}
