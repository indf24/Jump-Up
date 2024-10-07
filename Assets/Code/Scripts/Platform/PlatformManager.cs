using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Start()
    {
        currentPlatform = GameObject.FindWithTag("BPlatform").GetComponent<Platform>();
        player = GameObject.Find("Player");
        CreatePool();
        SpawnPlatform();
    }

    // Creates a pool of platforms to use throughout the game
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

    // Spawns a platform from the pool if any are available, otherwise instantiate a new one and spawn that one
    public void SpawnPlatform() 
    {
        if (platformPool.Any())
        {
            nextPlatform = platformPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(platformPrefab);
            nextPlatform = obj.GetComponent<Platform>();
        }

        nextPlatform.Spawn(player.transform.position.x);
    }

    // Moves the platfrom where the player land downwards to a predefined position
    public IEnumerator MovePlatform() 
    {
        currentPlatform = nextPlatform;

        // Makes the player a child object of the current platform to make them move together
        player.transform.parent = currentPlatform.transform; 

        yield return new WaitForSeconds(0.5f);

        float currentYPos = currentPlatform.transform.position.y;
        float targetYPos = 3.5f;

        float speed = 15f;

        // Moves the platform down smoothly
        while (currentYPos > targetYPos) 
        {
            float moveDistance = speed * Time.deltaTime;
            currentYPos -= moveDistance;

            currentPlatform.Move(moveDistance);

            yield return null;
        }

        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        SpawnPlatform();
        playerManager.isTouchAllowed = true;
    }

    // Despawns the platform the player jumped from
    public IEnumerator DespawnPlatform() 
    {
        if (currentPlatform.CompareTag("BPlatform"))
        {
            currentPlatform.Despawn();
        }
        else
        {
            animator = currentPlatform.GetComponent<Animator>();
            animator.SetTrigger("Despawn");

            // Wait for the despawn animation to end
            yield return new WaitForSeconds(0.1f);

            currentPlatform.Despawn();
            platformPool.Enqueue(currentPlatform);
        }
    }
}
