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

    // Reloads the current game mode scene
    private void Restart()
    {
        sceneControl.ReloadScene();
    }

    // Loads to the main menu scene
    private void GoToMenu()
    {
        sceneControl.LoadScene("MainMenu");
    }

    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameOverButtons.SetActive(true);
        retryButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(GoToMenu);
        scoreManager.UpdateHighScore();
    }
}
