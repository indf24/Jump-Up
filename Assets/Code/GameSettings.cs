using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    internal static GameSettings instance;
    private GameObject tutorial;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyGameSettings();
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyGameSettings();

        if (SceneManager.GetActiveScene().buildIndex is 1)
        {
            tutorial = GameObject.Find("Tutorial");
            StartCoroutine(ShowTutorial());
        }
    }

    private void ApplyGameSettings()
    {
# if UNITY_EDITOR
        Application.targetFrameRate = 400;
#else
        Application.targetFrameRate = 60;
#endif
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(1080, 1920, true);
    }

    private IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(3f);

        StartCoroutine(Utils.ChangeOpacityOverTime(tutorial.GetComponent<SpriteRenderer>(), 0.4f, 1f));
    }

    internal void HideTutorial() => tutorial.SetActive(false);

    internal IEnumerator CoverScreen()
    {
        Debug.Log("Cover Screen");

        GameObject cover = GameObject.Find("UI").transform.Find("Cover").gameObject;
        Debug.Log(cover);
        cover.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        cover.SetActive(false);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            StartCoroutine(CoverScreen());
        }
    }
}