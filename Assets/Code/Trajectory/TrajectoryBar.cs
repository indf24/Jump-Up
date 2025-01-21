using UnityEngine;

public class TrajectoryBar : MonoBehaviour
{
    // Spawns a dot
    public void Spawn(Vector2 spawnPosition)
    {
        gameObject.transform.position = spawnPosition;
        gameObject.SetActive(true);
    }

    // Despawns a dot
    public void Despawn() => gameObject.SetActive(false);
}
