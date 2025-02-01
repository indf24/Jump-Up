using System.Collections;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Transform sprite;
    [SerializeField] private Animator animator;

    public static bool PlayerInputAllowed { get; set; } = false;

    private RaycastHit2D hit;

    private void Start() => EnableInput();

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

    private void Update() => sprite.rotation = Quaternion.identity;

    // Methods to safely modify the value
    public static void EnableInput() => PlayerInputAllowed = true;
    public static void DisableInput() => PlayerInputAllowed = false;

    // Stops the player
    private void StopMovement()
    { 
        playerRb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        playerRb.velocity = Vector2.zero;
    }

    // Checks if the player is in contact the top of a platform  
    private bool IsGrounded() => hit.collider != null && hit.collider.CompareTag("Platform");

    // Creates a raycast for detection of contact between the player and a platform
    private void CreateGroudedRay()
    {
        Vector2 rayOrigin = playerRb.position - new Vector2(0, (playerCollider.bounds.size.y / 2) + 0.002f);
        float rayDistance = 0.1f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);

        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, Color.red, 0.1f);
    }

    private void StartPlatformCollision() => StartCoroutine(PlatformCollision());

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

    private void PlayerAnimation(string animation, bool state) => animator.SetBool(animation, state);
}
