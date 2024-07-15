using UnityEngine;

public class Platform : MonoBehaviour
{
    private char spawnSide;
    private float nextPlatformOffset = 2f;

    private float xSpawnPos;
    private float xSpawnPosRange = 4f;

    private float ySpawnPos;
    private float ySpawnPosMin = 8f;
    private float ySpawnPosMax = 15f;

    public void Spawn(float playerXPosition)
    {
        // Activate the next platform at a random position
        gameObject.transform.position = SpawnPosition(playerXPosition);
        gameObject.SetActive(true);
    }

    private Vector2 SpawnPosition(float playerXPosition)
    {
        // Spawn side selection regarding the platform where the ball is
        if (playerXPosition + nextPlatformOffset > xSpawnPosRange)
        {
            spawnSide = 'l';
        }
        else if (playerXPosition - nextPlatformOffset < -xSpawnPosRange)
        {
            spawnSide = 'r';
        }
        else
        {
            spawnSide = Random.Range(0, 2) == 0 ? 'l' : 'r';
        }

        // Next platform x axis spawn
        if (spawnSide == 'l')
        {
            xSpawnPos = Random.Range(-xSpawnPosRange, playerXPosition - nextPlatformOffset);
        }
        else
        {
            xSpawnPos = Random.Range(playerXPosition + nextPlatformOffset, xSpawnPosRange);
        }

        // Next platform y axis spawn
        ySpawnPos = Random.Range(ySpawnPosMin, ySpawnPosMax);

        return new Vector2(xSpawnPos, ySpawnPos);
    }

    public void Despawn()
    {
        // Deactivates the platform
        gameObject.SetActive(false);
    }

    public void Move(float moveDistance)
    {
        gameObject.transform.Translate(Vector2.down * moveDistance);
    }
}
