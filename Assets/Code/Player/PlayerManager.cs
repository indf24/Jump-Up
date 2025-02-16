using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    internal static PlayerManager instance;

    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Transform sprite;
    [SerializeField] private Animator animator;

    internal static bool PlayerInputAllowed { get; set; } = false;

    private RaycastHit2D hit;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        EnableInput();
    }

    private void Update() => sprite.rotation = Quaternion.identity;

    internal static void EnableInput() => PlayerInputAllowed = true;
    internal static void DisableInput() => PlayerInputAllowed = false;

    internal GameObject GetPlayerObject() => playerRb.gameObject;

    internal void ResetPlayer()
    {
        FreezePlayer();
        playerRb.transform.position = new(0f, -1.25f);
        playerRb.transform.SetParent(PlatformManager.instance.bottomPlatform.transform);
    }

    internal void FreezePlayer()
    {
        playerRb.velocity = Vector2.zero;
        playerRb.rotation = 0;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    internal void UnfreezePlayer() => playerRb.constraints = RigidbodyConstraints2D.None;

    // Checks if the player is in contact the top of a platform  
    private bool IsGrounded() => hit.collider != null && hit.collider.CompareTag("Platform");

    // Creates a raycast for detection of contact between the player and a platform
    private void CreateGroudedRay()
    {
        Vector2 rayOrigin = playerRb.position - new Vector2(0, (playerCollider.bounds.size.y / 2) + 0.001f);
        float rayDistance = 0.01f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);

        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, Color.red);
    }

    internal void StartPlatformCollision() => StartCoroutine(PlatformCollision());

    private IEnumerator PlatformCollision()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        PlayerAnimation("Flying", false);

        while (elapsedTime < duration)
        {
            CreateGroudedRay();

            if (IsGrounded())
            {
                FreezePlayer();
                GameCoordinator.instance.PlayerLand();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    internal void PlayerAnimation(string animation, bool state) => animator.SetBool(animation, state);
}
