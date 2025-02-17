using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private readonly float nextPlatformOffset = 5f;

    private const float xSpawnPosRange = 6f;

    private readonly float ySpawnPosMin = 11.75f;
    private readonly float ySpawnPosMax = 23.75f;

    internal enum Side { Left, Right }

    // Gets a random position from SpawnPosition() and spawn the platform there
    internal void Spawn(float playerXPos)
    {
        transform.position = SpawnPosition(playerXPos);
        gameObject.SetActive(true);
    }

    // Returns a random position for the next platform
    private Vector2 SpawnPosition(float playerXPos)
    {
        Side spawnSide = SpawnSide(playerXPos);

        // Chooses a random number for the x axis position of the next platform. Includes an offset to remove impossible jumps
        float xSpawnPos = spawnSide switch
        {
            Side.Left => Random.Range(-xSpawnPosRange, playerXPos - nextPlatformOffset),
            Side.Right => Random.Range(playerXPos + nextPlatformOffset, xSpawnPosRange)
        };

        float ySpawnPos = Random.Range(ySpawnPosMin, ySpawnPosMax);

        return new Vector2(xSpawnPos, ySpawnPos);
    }

    private Side SpawnSide(float playerXPos)
    {
        int left = 0, right = 2;

        // Checks how close the player is to the borders so the platforms don't spawn out of bounds
        return playerXPos switch
        {
            float p when p + nextPlatformOffset > xSpawnPosRange => Side.Left,
            float p when p - nextPlatformOffset < -xSpawnPosRange => Side.Right,
            _ => Random.Range(left, right) == 0 ? Side.Left : Side.Right,
        };
    }

    private Side GetSide() => gameObject.transform.position.x < 0 ? Side.Left : Side.Right;

    // Despawns the platform
    internal IEnumerator Despawn()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Despawn");
        float despawnTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(despawnTime);

        gameObject.SetActive(false);
    }

    internal void Move(Vector2 targetPos, float duration) => StartCoroutine(Utils.MoveObject(gameObject, targetPos, duration));

    internal void Difficulty(int diff)
    {
        if (diff > 0)
        {
            StartCoroutine(PingPongMove(diff));
        }
    }

    internal void Stop() => StopAllCoroutines();

    private IEnumerator PingPongMove(float diff)
    {
        Side side = GetSide();
        float platformXPos = transform.position.x;
        float distance = 2;
        float leftRange = 0f;
        float rightRange = 0f;

        switch (side)
        {
            case Side.Left:
                leftRange = Mathf.Max(platformXPos - distance, -xSpawnPosRange);
                rightRange = leftRange + (2 * distance);
                break;

            case Side.Right:
                rightRange = Mathf.Min(platformXPos + distance, xSpawnPosRange);
                leftRange = rightRange - (2 * distance);
                break;
        }

        float t = 0;
        float direction = 1;
        diff /= 5;

        while (true)
        {
            t += direction * diff * Time.deltaTime;
            t = Mathf.Clamp01(t);

            float xPos = Mathf.Lerp(leftRange, rightRange, t);
            transform.position = new(xPos, transform.position.y);

            if (t is >= 1 or <= 0)
            {
                direction *= -1;
            }

            yield return null;
        }
    }

    internal void Show() => gameObject.SetActive(true);

    internal void Hide() => gameObject.SetActive(false);
}
