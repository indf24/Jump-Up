using UnityEngine;

public class TrajectoryDot : MonoBehaviour
{
    public void Spawn(Vector2 spawnPosition)
    {
        gameObject.transform.position = spawnPosition;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
