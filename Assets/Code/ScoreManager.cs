using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    internal static ScoreManager instance;

    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private int score;
    private int highscore;

    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI bestScore;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    // Adds points to the score
    internal void AddPoints(int points)
    {
        score += points;
        UpdateScore();
    }

    // Updates the score counter in the screen
    private void UpdateScore() => currentScore.text = score.ToString();

    // Update the high score on game over
    internal void UpdateHighscore()
    {
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
        }
    }

    internal void UpdateGameOverScores()
    {
        finalScore.text = score.ToString();
        bestScore.text = highscore.ToString();
    }

    internal void ResetCurrentScore() => currentScore.text = "0";

    internal int GetScore() => score;
}