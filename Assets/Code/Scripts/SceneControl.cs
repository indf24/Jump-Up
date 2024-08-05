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

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator DisableTouchBetweenScenes()
    {
        playerManager.isTouchAllowed = false;
        yield return new WaitForSeconds(0.2f);
        playerManager.isTouchAllowed = true;
    }
}
