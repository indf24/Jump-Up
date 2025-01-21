using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoad;

    // Reloads the current scene
    public static void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Loads a scene
    public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) => PlayerManager.EnableInput();
}
