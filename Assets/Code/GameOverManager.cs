using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        retryButton.onClick.AddListener(() => SceneControl.ReloadScene());
        menuButton.onClick.AddListener(() => SceneControl.LoadScene("MainMenu"));
    }

    private void ShowGameOverScreen()
    {
        gameOverButtons.SetActive(true);
    }

    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShowGameOverScreen();
        //EventHub.GameOver();
    }

}
