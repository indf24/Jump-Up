using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;

    private GameObject player;
    [SerializeField] private GameObject platformPrefab;
    private int poolSize = 2;
    private Queue<Platform> platformPool = new();

    private Platform currentPlatform;
    private Platform nextPlatform;

    private Animator animator;

    private void Awake()
    {
        player = GameObject.Find("Player");
        CreatePool();
    }

    private void Start()
    {
        currentPlatform = GameObject.FindWithTag("BPlatform").GetComponent<Platform>();
        SpawnPlatform();
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
            nextPlatform = platformPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(platformPrefab);
            nextPlatform = obj.GetComponent<Platform>();
        }

        nextPlatform.Spawn();
    }

    public IEnumerator MovePlatform()
    {
        currentPlatform = nextPlatform; //Designate the platform where the player is as the current one
        animator = currentPlatform.GetComponent<Animator>(); //Get the current platforms animator

        player.transform.parent = currentPlatform.transform; //Make the player a child object of the current platform to move together

        yield return new WaitForSeconds(0.5f);

        float currentYPos = currentPlatform.transform.position.y;
        float targetYPos = 3.5f;

        float speed = 15f;

        while (currentYPos > targetYPos) // Move the platform down smoothly
        {
            float moveDistance = speed * Time.deltaTime;
            currentYPos -= moveDistance;

            currentPlatform.transform.position += Vector3.down * moveDistance;

            yield return null;
        }

        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        SpawnPlatform();
        playerManager.canJump = true;
    }

    public IEnumerator DespawnPlatform()
    {
        if (currentPlatform.CompareTag("BPlatform"))
        {
            // Despawn the first platform
            currentPlatform.Despawn();
        }
        else
        {
            // Play despawn animation and despawn the platform
            animator.SetTrigger("Despawn");
            yield return new WaitForSeconds(0.2f);
            currentPlatform.Despawn();
            platformPool.Enqueue(currentPlatform);
        }
    }
}
