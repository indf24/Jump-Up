using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        StartCoroutine(DisableTouchBetweenScenes());
    }

    // Reloads the current scene
    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


    // Loads a scene
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Disables player input on scene transition to stop unintentional player movement
    public IEnumerator DisableTouchBetweenScenes()
    {
        playerManager.isTouchAllowed = false;
        yield return new WaitForSeconds(0.2f);
        playerManager.isTouchAllowed = true;
    }
}
