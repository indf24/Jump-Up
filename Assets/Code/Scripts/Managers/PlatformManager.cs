using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    private int poolSize = 2;
    private Queue<Platform> platformPool = new();
    Platform platform;
    Animator animator;

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(platformPrefab);
            Platform platform = obj.GetComponent<Platform>();
            platform.Despawn();
            platformPool.Enqueue(platform);
        }
    }

    public void SpawnPlatform()
    {
        if (platformPool.Count > 0)
        {
            platform = platformPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(platformPrefab);
            platform = obj.GetComponent<Platform>();
        }

        animator = platform.GetComponent<Animator>();
        platform.Spawn();
    }

    public IEnumerator DespawnPlatform()
    {
        Platform platform = this.platform;

        animator.SetTrigger("Despawn");
        yield return new WaitForSeconds(0.2f);
        platform.Despawn();
        platformPool.Enqueue(platform);
    }
}
