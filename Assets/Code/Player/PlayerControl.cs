using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D player;

    Touch touch = new();

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float minJumpForce = 100f;
    private float maxJumpForce = 800f;

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
            if (jumpVector.magnitude > minJumpForce)
            {
                if (touch.phase is TouchPhase.Moved)
                {
                    jumpVector = LimitJumpForce(jumpVector, minJumpForce, maxJumpForce);
                    EventHub.PlayerAim(touch, player, jumpVector, trajectorySteps);
                }

                if (touch.phase is TouchPhase.Ended)
                {
                    jumpVector = LimitJumpForce(jumpVector, minJumpForce, maxJumpForce);
                    Jump(jumpVector, minJumpForce, maxJumpForce);
                    PlayerManager.DisableInput();
                    EventHub.PlayerJump();
                }
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

        if (touch.phase is TouchPhase.Moved or TouchPhase.Ended)
        {
            endTouchPosition = touch.position;
            jumpVector = startTouchPosition - endTouchPosition;
        }

        return jumpVector;
    }

    private Vector2 LimitJumpForce(Vector2 jumpVector, float minJumpForce, float maxJumpForce)
    {
        return jumpVector.normalized * Mathf.Clamp(jumpVector.magnitude, minJumpForce, maxJumpForce);
    }

    private void Jump(Vector2 jumpVector, float minJumpForce, float maxJumpForce)
    {
        // Apply the force to the ball
        player.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EventHub.PlatformCollision();
    }
}