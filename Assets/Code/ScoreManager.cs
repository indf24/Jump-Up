using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScore;
    private int score;
    private int highscore;

    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI bestScore;

    private void Start()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void OnEnable()
    {
        EventHub.OnScoreEarned += AddPoints;
        EventHub.OnGameOver += UpdateHighscore;
        EventHub.OnGameOver += UpdateGameOverScores;
        EventHub.OnRetry += ResetCurrentScore;
    }

    private void OnDisable()
    {
        EventHub.OnScoreEarned -= AddPoints;
        EventHub.OnGameOver -= UpdateHighscore;
        EventHub.OnGameOver -= UpdateGameOverScores;
        EventHub.OnRetry -= ResetCurrentScore;
    }

    private void OnDestroy()
    {
        EventHub.OnScoreEarned -= AddPoints;
        EventHub.OnGameOver -= UpdateHighscore;
        EventHub.OnGameOver -= UpdateGameOverScores;
        EventHub.OnRetry -= ResetCurrentScore;
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
        currentScore.text = score.ToString();
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

    private void UpdateGameOverScores()
    {
        finalScore.text = score.ToString();
        bestScore.text = highscore.ToString();
    }

    private void ResetCurrentScore()
    {
        currentScore.text = "0";
    }
}