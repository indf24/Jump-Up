using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlatformManager platformManager;

    private Rigidbody2D player;

    public bool canJump = true;

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    private void StopMovement()
    {
        // Stop the ball
        player.constraints = RigidbodyConstraints2D.FreezePositionX;
        player.velocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        // Check if the player is touching the top of the platform
        Vector2 rayOrigin = player.position - new Vector2(0, (player.GetComponent<Collider2D>().bounds.size.y / 2) + 0.002f);
        float rayDistance = 0.03f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);
        return hit.collider != null && hit.collider.CompareTag("Platform");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            StopMovement();
            StartCoroutine(platformManager.MovePlatform());
        }
    }
}
