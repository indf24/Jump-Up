using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int score;

    private void Start()
    {
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
}