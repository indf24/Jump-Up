using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
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

    private void OnEnable()
    {
        EventHub.OnPlayerLanding += StartMovePlatform;
        EventHub.OnPlayerJump += StartDespawnPlatform;
    }

    private void OnDisable()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= StartDespawnPlatform;
    }

    private void OnDestroy()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= StartDespawnPlatform;
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
    private void SpawnPlatform() 
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

    private void StartMovePlatform()
    {
        StartCoroutine(MovePlatform());
    }

    // Moves the platfrom where the player land downwards to a predefined position
    public IEnumerator MovePlatform() 
    {
        currentPlatform = nextPlatform;

        // Makes the player a child object of the current platform to make them move together
        player.transform.parent = currentPlatform.transform; 

        yield return new WaitForSeconds(0.5f);

        Vector2 targetPos = new Vector2(currentPlatform.transform.position.x, 5.75f);
        float speed = 15f;

        yield return StartCoroutine(currentPlatform.Move(targetPos, speed));

        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionX;

        SpawnPlatform();
        PlayerManager.EnableInput();
    }

    private void StartDespawnPlatform()
    {
        StartCoroutine(DespawnPlatform());
    }

    // Despawns the platform the player jumped from
    public IEnumerator DespawnPlatform() 
    {
        if (currentPlatform.CompareTag("BPlatform"))
        {
            Vector2 targetPos = new Vector2(currentPlatform.transform.position.x, -1);
            float speed = 12f;

            yield return StartCoroutine(currentPlatform.Move(targetPos, speed));

            currentPlatform.Despawn();
        }
        else
        {
            animator = currentPlatform.GetComponent<Animator>();
            animator.SetTrigger("Despawn");

            // Wait for the despawn animation to end
            yield return new WaitForSeconds(0.096f);

            currentPlatform.Despawn();
            platformPool.Enqueue(currentPlatform);
        }
    }
}
