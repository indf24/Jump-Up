using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private TrajectoryManager trajectoryManager;

    private Rigidbody2D player;

    Touch touch = new();

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float minJumpForce = 600f;
    private float maxJumpForce = 1350f;

    private int trajectorySteps = 1000;

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Manages the entire player control by detecting screen touch
    void Update()
    {
        if (PlayerManager.PlayerInputAllowed && Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Vector2 jumpVector = CalculateJump(touch);

            // Enables the jump if its force is above a threshold
            if (jumpVector.y > minJumpForce)
            {
                EventHub.PlayerAnimation("Holding", true);

                jumpVector = LimitJumpForce(jumpVector);

                if (touch.phase is TouchPhase.Moved)
                {              
                    trajectoryManager.MakeTrajectory(touch, player, jumpVector, trajectorySteps);
                }

                if (touch.phase is TouchPhase.Ended)
                {
                    trajectoryManager.DespawnDots();
                    Jump(jumpVector);
                    PlayerManager.DisableInput();
                    EventHub.PlayerJump();
                    EventHub.PlayerAnimation("Flying", true);
                    EventHub.PlayerAnimation("Holding", false);
                }
            }
            else
            {
                trajectoryManager.DespawnDots();
                EventHub.PlayerAnimation("Holding", false);
            }
        }
    }

    private Vector2 CalculateJump(Touch touch)
    {
        Vector2 jumpVector = Vector2.zero;

        if (touch.phase is TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }
        else
        {
            endTouchPosition = touch.position;
            jumpVector = startTouchPosition - endTouchPosition;
            jumpVector = jumpVector.normalized * (jumpVector.magnitude + 300); //Change later
        }

        return jumpVector;
    }

    private Vector2 LimitJumpForce(Vector2 jumpVector)
    {
        return jumpVector.normalized * Mathf.Clamp(jumpVector.magnitude, minJumpForce, maxJumpForce);
    }

    private void Jump(Vector2 jumpVector)
    {
        // Apply the force to the ball
        player.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            EventHub.PlatformCollision();
        }
    }
}