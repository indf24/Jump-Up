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

    public void AddPoints(int points)
    {
        score += points;
        UpdateScore(score);
    }

    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
}