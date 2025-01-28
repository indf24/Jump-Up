using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject platformPrefab;
    private Queue<Platform> platformPool = new();

    [SerializeField] private Platform bottomPlatform;
    private Platform currentPlatform;
    private Platform nextPlatform;

    private bool MovePlatformActive = false;

    private void Start()
    {
        currentPlatform = bottomPlatform;
        CreatePool();
        SpawnPlatform();
    }

    private void OnEnable()
    {
        EventHub.OnPlayerLanding += StartMovePlatform;
        EventHub.OnPlayerJump += DespawnPlatform;
        EventHub.OnGameOver += GameOver;
        EventHub.OnRetry += ShowBottomPatform;
        EventHub.OnSecondChance += StartSecondChance;
    }

    private void OnDisable()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= DespawnPlatform;
        EventHub.OnGameOver -= GameOver;
        EventHub.OnRetry -= ShowBottomPatform;
        EventHub.OnSecondChance -= StartSecondChance;
    }

    private void OnDestroy()
    {
        EventHub.OnPlayerLanding -= StartMovePlatform;
        EventHub.OnPlayerJump -= DespawnPlatform;
        EventHub.OnGameOver -= GameOver;
        EventHub.OnRetry -= ShowBottomPatform;
        EventHub.OnSecondChance -= StartSecondChance;
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
        if (!MovePlatformActive)
        {
            StartCoroutine(MovePlatform());
        }
    }

    // Moves the platfrom where the player land downwards to a predefined position
    public IEnumerator MovePlatform() 
    {
        MovePlatformActive = true;

        currentPlatform = nextPlatform;

        // Makes the player a child object of the current platform to make them move together
        player.transform.parent = currentPlatform.transform; 

        yield return new WaitForSeconds(0.5f);

        Vector2 targetPos = new(currentPlatform.transform.position.x, 5.75f);
        float duration = 0.7f;

        currentPlatform.Move(targetPos, duration);
        yield return new WaitForSeconds(duration);

        player.transform.parent = null;
        player.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionX;

        SpawnPlatform();
        PlayerManager.EnableInput();

        MovePlatformActive = false;
    }

    // Despawns the platform the player jumped from
    private void DespawnPlatform() 
    {
        if (currentPlatform == bottomPlatform)
        {
            StartCoroutine(HideBottomPlatform());
        }   
        else
        {
            StartCoroutine(currentPlatform.Despawn());
            platformPool.Enqueue(currentPlatform);
        }
    }

    private IEnumerator HideBottomPlatform()
    {
        Vector2 targetPos = new(bottomPlatform.transform.position.x, -2f);
        bottomPlatform.Move(targetPos, 0.7f);
        yield return new WaitForSeconds(0.7f);
        bottomPlatform.gameObject.SetActive(false);
    }

    private void StartSecondChance() => StartCoroutine(SecondChance());

    private IEnumerator SecondChance()
    {
        ShowBottomPatform();
        yield return new WaitForSeconds(2f);
        SpawnPlatform();
    }

    private void GameOver()
    {
        StartCoroutine(nextPlatform.Despawn());
        bottomPlatform.gameObject.SetActive(true);
    }

    private void ShowBottomPatform()
    {
        Vector2 targetPos = new(bottomPlatform.transform.position.x, 5.75f);
        bottomPlatform.Move(targetPos, 0.7f);
    } 
}
