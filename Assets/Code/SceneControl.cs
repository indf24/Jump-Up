using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(gameObject);

    // Reloads the current scene
    public static void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Loads a scene
    public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
