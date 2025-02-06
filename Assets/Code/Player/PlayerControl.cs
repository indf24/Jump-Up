using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private const int minJumpForce = 150;
    private const int maxJumpForce = 1350;

    // Manages the entire player control by detecting screen touch
    void Update()
    {
        if (Input.touchCount == 0 || !PlayerManager.PlayerInputAllowed) return;

        Touch touch = Input.GetTouch(0);
        Vector2 jumpVector = CalculateJump(touch);

        if (jumpVector.y < minJumpForce)
        {
            TrajectoryManager.instance.DespawnBars();
            PlayerManager.instance.PlayerAnimation("Holding", false);
            return;
        }

        PlayerManager.instance.PlayerAnimation("Holding", true);

        jumpVector = LimitJumpForce(jumpVector);

        switch (touch.phase)
        {
            case TouchPhase.Moved:
                TrajectoryManager.instance.MakeTrajectory(player, jumpVector, minJumpForce, maxJumpForce);
                break;

            case TouchPhase.Ended:
                TrajectoryManager.instance.DespawnBars();
                Jump(jumpVector);

                PlayerManager.DisableInput();

                PlatformManager.instance.DespawnPlatform();
                TrajectoryManager.instance.DespawnBars();

                PlayerManager.instance.PlayerAnimation("Flying", true);
                PlayerManager.instance.PlayerAnimation("Holding", false);
                break;
        }
    }

    private Vector2 CalculateJump(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                return Vector2.zero;

            case TouchPhase.Stationary:
            case TouchPhase.Moved:
            case TouchPhase.Ended:
                endTouchPosition = touch.position;
                return (startTouchPosition - endTouchPosition) * 3;

            default:
                return Vector2.zero;
        }
    }

    private Vector2 LimitJumpForce(Vector2 jumpVector) => jumpVector.normalized * Mathf.Clamp(jumpVector.magnitude, minJumpForce, maxJumpForce);

    private void Jump(Vector2 jumpVector) => player.AddForce(jumpVector, ForceMode2D.Impulse);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Platform")) return;

        PlayerManager.instance.PlayerAnimation("Holding", false);
        PlayerManager.instance.PlayerAnimation("Flying", false);
        PlayerManager.instance.StartPlatformCollision();
    }
}