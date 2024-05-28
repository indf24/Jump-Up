using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private float xSpawnPos = 7f;
    private float ySpawnPos;
    private float ySpawnPosRange = 4f;

    // Move the obstacle to the spawn position at a random height and activate it
    public void Spawn()
    {
        ySpawnPos = Random.Range(-ySpawnPosRange, ySpawnPosRange);
        gameObject.transform.position = new Vector2(xSpawnPos, ySpawnPos);
        gameObject.SetActive(true);
    }

    // Deactivates the obstacle
    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
