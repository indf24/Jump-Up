using UnityEngine;

public class Platform : MonoBehaviour
{
    private char spawnSide;
    private float nextPlatformOffset = 5f;

    private float xSpawnPos;
    private float xSpawnPosRange = 6.5f;

    private float ySpawnPos;
    private float ySpawnPosMin = 11.75f;
    private float ySpawnPosMax = 23.75f;

    // Gets a random position from SpawnPosition() and spawn the platform there
    public void Spawn(float playerXPosition) 
    {
        gameObject.transform.position = SpawnPosition(playerXPosition);
        gameObject.SetActive(true);
    }

    // Returns a random position for the next platform
    private Vector2 SpawnPosition(float playerXPosition) 
    {
        // Checks how close the player is to the borders so the platforms don't spawn outside the game zone
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

        // Chooses a random number for the x axis position of the next platform. Includes an offset to remove impossible jumps
        if (spawnSide == 'l')
        {
            xSpawnPos = Random.Range(-xSpawnPosRange, playerXPosition - nextPlatformOffset);
        }
        else
        {
            xSpawnPos = Random.Range(playerXPosition + nextPlatformOffset, xSpawnPosRange);
        }

        ySpawnPos = Random.Range(ySpawnPosMin, ySpawnPosMax);

        return new Vector2(xSpawnPos, ySpawnPos);
    }


    // Despawns the platform
    public void Despawn()
    {
        // Deactivates the platform
        gameObject.SetActive(false);
    }

    // Moves the platform downwards
    public void Move(float moveDistance)
    {
        gameObject.transform.Translate(Vector2.down * moveDistance);
    }
}
