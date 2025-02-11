using System;
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
        if (Input.touchCount == 0 || !PlayerManager.PlayerInputAllowed)
            return;

        Touch touch = Input.GetTouch(0);
        Vector2 jumpVector = CalculateJump(touch);

        if (jumpVector.y < minJumpForce)
        {
            GameCoordinator.instance.PlayerCancelJump();
            return;
        }

        GameCoordinator.instance.PlayerPrepareJump();

        jumpVector = AdjustJumpForce(jumpVector);

        switch (touch.phase)
        {
            case TouchPhase.Moved:
                GameCoordinator.instance.UpdateTrajectory(player, jumpVector, minJumpForce, maxJumpForce);
                break;

            case TouchPhase.Ended:
                Jump(jumpVector);

                PlayerManager.DisableInput();

                GameCoordinator.instance.PlayerJump();
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

    private Vector2 AdjustJumpForce(Vector2 jumpVector)
    {
        const int steps = 10;

        float rawMagnitude = jumpVector.magnitude;
        float clampedMagnitude = Mathf.Clamp(rawMagnitude, minJumpForce, maxJumpForce);

        float stepSize = (maxJumpForce - minJumpForce) / steps;
        int steppedMagnitude = Mathf.RoundToInt(minJumpForce + (Mathf.Floor((clampedMagnitude - minJumpForce) / stepSize) * stepSize));

        return jumpVector.normalized * steppedMagnitude;
    }

    private void Jump(Vector2 jumpVector) => player.AddForce(jumpVector, ForceMode2D.Impulse);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Platform"))
            return;

        GameCoordinator.instance.PlayerPlatformCollision();
    }
}