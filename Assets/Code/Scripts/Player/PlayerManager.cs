using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlatformManager platformManager;
    [SerializeField] private ScoreManager scoreManager;

    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    public bool canJump = true;
    public bool alreadyGrounded = false;

    private RaycastHit2D hit;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (IsGrounded() && !alreadyGrounded)
        {
            StopMovement();
            alreadyGrounded = true;
            scoreManager.AddPoints(1);
            StartCoroutine(platformManager.MovePlatform());
        }
    }

    private void StopMovement()
    {
        // Stop the ball
        playerRb.constraints = RigidbodyConstraints2D.FreezePositionX;
        playerRb.velocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        // Check if the player is touching the top of the platform  
        CreateGroudedRay();
        return hit.collider != null && hit.collider.CompareTag("Platform");
    }

    private void CreateGroudedRay()
    {
        Vector2 rayOrigin = playerRb.position - new Vector2(0, (playerCollider.bounds.size.y / 2) + 0.002f);
        float rayDistance = 0.03f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);
    }

    public IEnumerator NotGrounded()
    {
        yield return new WaitForSeconds(0.2f);
        alreadyGrounded = false;
    }
}
