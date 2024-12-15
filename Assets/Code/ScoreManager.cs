using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int score;
    private int highscore;

    [SerializeField] private TextMeshProUGUI highscoreText;

    private void Start()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        scoreText = GameObject.Find("PlayerScore").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventHub.OnScoreEarned += AddPoints;
        EventHub.OnGameOver += UpdateHighscore;
        EventHub.OnGameOver += UpdateUIHighscore;
    }

    private void OnDisable()
    {
        EventHub.OnScoreEarned -= AddPoints;
        EventHub.OnGameOver -= UpdateHighscore;
        EventHub.OnGameOver -= UpdateUIHighscore;
    }

    private void OnDestroy()
    {
        EventHub.OnScoreEarned -= AddPoints;
        EventHub.OnGameOver -= UpdateHighscore;
        EventHub.OnGameOver -= UpdateUIHighscore;
    }

    // Adds points to the score
    private void AddPoints(int points)
    {
        score += points;
        UpdateScore();
    }

    // Updates the score counter in the screen
    private void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    // Update the high score on game over
    private void UpdateHighscore()
    {
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
        }
    }

    private void UpdateUIHighscore()
    {
        highscoreText.text = highscore.ToString();
    }
}