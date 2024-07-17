using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private SceneControl sceneControl;
    [SerializeField] private ScoreManager scoreManager;

    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        //gameOverButtons = GameObject.Find("GameOverButtons");
    }

    private void Restart()
    {
        sceneControl.ReloadScene();
    }

    private void GoToMenu()
    {
        sceneControl.LoadScene("MainMenu");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameOverButtons.SetActive(true);
        retryButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(GoToMenu);
        scoreManager.UpdateHighScore();
    }
}
