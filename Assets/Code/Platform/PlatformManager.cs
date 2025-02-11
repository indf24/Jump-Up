using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    internal static PlatformManager instance;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject platformPrefab;
    private Queue<Platform> platformPool = new();

    [SerializeField] internal Platform bottomPlatform;
    private Platform currentPlatform;
    private Platform nextPlatform;

    private bool MovePlatformActive = false;

    private const float bottomPlatformXPos = 0;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        currentPlatform = bottomPlatform;
        CreatePool();
        SpawnPlatform();
    }

    // Creates a pool of platforms to use throughout the game
    private void CreatePool()
    {
        int poolSize = 2;

        for (int i = 0; i < poolSize; i++)
        {
            Platform platform = Instantiate(platformPrefab).GetComponent<Platform>();
            platform.gameObject.SetActive(false);
            platformPool.Enqueue(platform);
        }
    }

    private Platform GetPlatform() => platformPool.Any() ? platformPool.Dequeue() : Instantiate(platformPrefab).GetComponent<Platform>();

    // Spawns a platform from the pool if any are available, otherwise instantiate a new one and spawn that one
    private void SpawnPlatform()
    {
        float playerXPos = player.transform.position.x;

        nextPlatform = GetPlatform();
        nextPlatform.Spawn(playerXPos);
    }

    internal void StartNextPlatformSequence() => StartCoroutine(NextPlatformSequence());

    private IEnumerator NextPlatformSequence()
    {
        if (MovePlatformActive)
            yield break;

        currentPlatform = nextPlatform;

        currentPlatform.Stop();

        // Makes the player a child object of the current platform to make them move together
        player.transform.SetParent(currentPlatform.transform);

        yield return new WaitForSeconds(0.5f);

        MovePlatformActive = true;
        yield return StartCoroutine(MovePlatform());
        MovePlatformActive = false;

        player.transform.parent = null;
        PlayerManager.instance.UnfreezePlayer();

        SpawnPlatform();
        SetDifficulty();
        PlayerManager.EnableInput();
    }

    private void SetDifficulty()
    {
        int diff = ScoreManager.instance.GetScore() switch
        {
            < 20 => 0,
            >= 20 and < 30 => 1,
            >= 30 and < 40 => 2,
            >= 40 and < 50 => 3,
            >= 50 => 4
        };

        nextPlatform.Difficulty(diff);
    }

    // Moves the platfrom where the player land downwards to a predefined position
    private IEnumerator MovePlatform()
    {
        float playerXPos = currentPlatform.transform.position.x;
        float duration = 0.7f;

        Vector2 targetPos = new(playerXPos, 5.75f);
        currentPlatform.Move(targetPos, duration);

        yield return new WaitForSeconds(duration);
    }

    // Despawns the platform the player jumped from
    internal void DespawnPlatform()
    {
        if (currentPlatform == bottomPlatform)
        {
            StartCoroutine(HideBottomPlatform());
            return;
        }

        StartCoroutine(currentPlatform.Despawn());
        platformPool.Enqueue(currentPlatform);
    }

    private IEnumerator HideBottomPlatform()
    {
        Vector2 targetPos = new(bottomPlatformXPos, -2f);
        bottomPlatform.Move(targetPos, 0.7f);

        yield return new WaitForSeconds(0.7f);

        bottomPlatform.Hide();
    }

    internal void StartSecondChance() => StartCoroutine(SecondChance());

    private IEnumerator SecondChance()
    {
        yield return new WaitForSeconds(0.5f);

        currentPlatform = bottomPlatform;
        ShowBottomPlatform();

        yield return new WaitForSeconds(2f);

        SpawnPlatform();
        SetDifficulty();
    }

    internal void GameOver()
    {
        StartCoroutine(nextPlatform.Despawn());
        bottomPlatform.Show();
    }

    internal void ShowBottomPlatform()
    {
        Vector2 targetPos = new(bottomPlatformXPos, 5.75f);
        bottomPlatform.Move(targetPos, 0.7f);
    }
}
