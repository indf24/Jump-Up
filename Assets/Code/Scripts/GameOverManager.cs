using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private SceneControl sceneControl;

    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        //gameOverButtons = GameObject.Find("GameOverButtons");
        retryButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(GoToMenu);
    }

    private void Restart()
    {
        sceneControl.ReloadScene();
    }

    private void GoToMenu()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameOverButtons.SetActive(true);
    }
}
