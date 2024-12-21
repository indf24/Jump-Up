using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private GameObject platformPrefab;
    private Queue<Platform> platformPool = new();

    private Platform bottomPlatform;
    private Platform currentPlatform;
    private Platform nextPlatform;

    private void Start()
    {
        bottomPlatform = GameObject.FindWithTag("BPlatform").GetComponent<Platform>();
        currentPlatform = bottomPlatform;
        player = GameObject.Find("Player");
        CreatePool();
        SpawnPlatform();
    }

    private void OnEnable()
    {
        EventHub.OnPlayerLanding += StartMovePlatform;
        EventHub.OnPlayerJump += DespawnPlatform;
        EventHub.OnGameOver += GameOver;
        EventHub.OnRetry += Restart;
    }

    private void OnDisable()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= DespawnPlatform;
        EventHub.OnGameOver -= GameOver;
        EventHub.OnRetry -= Restart;
    }

    private void OnDestroy()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= DespawnPlatform;
        EventHub.OnGameOver -= GameOver;
        EventHub.OnRetry -= Restart;
    }

    // Creates a pool of platforms to use throughout the game
    private void CreatePool() 
    {
        int poolSize = 2;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(platformPrefab);
            Platform platform = obj.GetComponent<Platform>();
            platform.gameObject.SetActive(false);
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

        Vector2 targetPos = new(currentPlatform.transform.position.x, 5.75f);
        float duration = 0.7f;

        yield return StartCoroutine(currentPlatform.Move(targetPos, duration));

        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionX;

        SpawnPlatform();
        PlayerManager.EnableInput();
    }

    // Despawns the platform the player jumped from
    private void DespawnPlatform() 
    {
        if (currentPlatform.CompareTag("BPlatform"))
        {
            MoveBottomPlatform(-2f, 1f);
        }
        else
        {
            StartCoroutine(currentPlatform.Despawn());
            platformPool.Enqueue(currentPlatform);
        }
    }

    private void MoveBottomPlatform(float targetYPos, float duration)
    {
        Vector2 targetPos = new(bottomPlatform.transform.position.x, targetYPos);

        StartCoroutine(bottomPlatform.Move(targetPos, duration));
    }

    private void GameOver()
    {
        StartCoroutine(nextPlatform.Despawn());
    }

    private void Restart()
    {
        MoveBottomPlatform(5.75f, 0.7f);
    } 
}
