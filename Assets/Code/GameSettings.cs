using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;

    private void Awake()
    {
        ApplyGameSettings();

        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyGameSettings();
    }

    private void ApplyGameSettings()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(1080, 1920, true);
    }
}