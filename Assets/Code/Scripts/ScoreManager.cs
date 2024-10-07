using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int score;
    private int highScore;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
    }

    // Adds points to the score
    public void AddPoints(int points)
    {
        score += points;
        UpdateScore(score);
    }

    // Updates the score counter in the screen
    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // Update the high score on game over
    public void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
}