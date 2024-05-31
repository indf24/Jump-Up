using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlatformManager platformManager;

    private void Start()
    {
        platformManager.SpawnPlatform();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(platformManager.DespawnPlatform());
            platformManager.SpawnPlatform();
        }
    }
}
