using UnityEngine;

public class Platform : MonoBehaviour
{
    private float currentXPos = 0;
    private char spawnSide;
    private float nextPlatformOffset = 1.5f;

    private float xSpawnPos;
    private float xSpawnPosRange = 4f;

    private float ySpawnPos;
    private float ySpawnPosMin = 8f;
    private float ySpawnPosMax = 16f;

    public void Spawn()
    {
        // Activate the next platform at a random position
        gameObject.transform.position = SpawnPosition();
        gameObject.SetActive(true);
    }

    private Vector2 SpawnPosition()
    {
        // Spawn side selection regarding the platform where the ball is
        if (currentXPos + nextPlatformOffset > xSpawnPosRange)
        {
            spawnSide = 'l';
        }
        else if (currentXPos - nextPlatformOffset < -xSpawnPosRange)
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
            xSpawnPos = Random.Range(-xSpawnPosRange, currentXPos - nextPlatformOffset);
        }
        else
        {
            xSpawnPos = Random.Range(currentXPos + nextPlatformOffset, xSpawnPosRange);
        }

        // Next platform y axis spawn
        ySpawnPos = Random.Range(ySpawnPosMin, ySpawnPosMax);

        return new Vector2(xSpawnPos, ySpawnPos);
    }

    public void Despawn()
    {
        // Deactivates the obstacle
        gameObject.SetActive(false);
    }
}
