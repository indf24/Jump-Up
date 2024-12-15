using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private TextMeshProUGUI highscore;

    [SerializeField] private GameObject ball;

    private void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(Retry()));
        menuButton.onClick.AddListener(() => SceneControl.LoadScene("MainMenu"));
    }

    private IEnumerator GameOverScreen()
    {
        yield return StartCoroutine(TurnPlayerObjectIntoUI());
        yield return StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator Retry()
    {
        HideGameOverUI();
        yield return StartCoroutine(ResetPlayerObject());

        SceneControl.ReloadScene();
    }

    private IEnumerator TurnPlayerObjectIntoUI()
    {
        ball.GetComponent<Rigidbody2D>().simulated = false;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Move(ball, new(0f, 15f), 0.7f));
        yield return StartCoroutine(Scale(ball, new(15f, 15f, 15f), 0.7f));
    }

    private IEnumerator ShowGameOverUI()
    {
        Color color = playerScoreText.color;
        color.a = 1f;

        yield return StartCoroutine(TextColor(playerScoreText, color, 0.5f));
        yield return StartCoroutine(MoveScore());
        StartCoroutine(TextColor(highscoreText, color, 0.5f));
        yield return StartCoroutine(TextColor(highscore, color, 0.5f));
        yield return new WaitForSeconds(0.5f);
        gameOverButtons.SetActive(true);
    }

    private IEnumerator MoveScore()
    {
        Color color = playerScoreText.color;

        StartCoroutine(UIMove(playerScore.rectTransform, new(225f, 50f), 0.7f));
        yield return StartCoroutine(TextColor(playerScore, color, 0.7f));
    }

    private void HideGameOverUI()
    {
        playerScoreText.gameObject.SetActive(false);
        playerScore.gameObject.SetActive(false);
        highscoreText.gameObject.SetActive(false);
        highscore.gameObject.SetActive(false);
        gameOverButtons.SetActive(false);
    }

    private IEnumerator ResetPlayerObject()
    {
        yield return StartCoroutine(Scale(ball, new(1f, 1f, 1f), 0.7f));
        EventHub.Retry();
        yield return StartCoroutine(Move(ball, new(0f, 6.5f), 0.7f));
    }

    private IEnumerator Move(GameObject obj, Vector2 targetPos, float duration)
    {
        Vector2 startPos = obj.transform.position;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easeT = 1 - Mathf.Pow(1 - t, 3);
            obj.transform.position = Vector2.Lerp(startPos, targetPos, easeT);
            yield return null;
        }

        obj.transform.position = targetPos;
    }

    private IEnumerator UIMove(RectTransform rectTransform, Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easeT = 1 - Mathf.Pow(1 - t, 3); // Cubic ease-out
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeT);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }

    private IEnumerator Scale(GameObject obj, Vector3 targetScale, float duration)
    {
        Vector3 startScale = obj.transform.localScale;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easeT = 1 - Mathf.Pow(1 - t, 3);
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, easeT);
            yield return null;
        }

        obj.transform.localScale = targetScale;
    }

    private IEnumerator TextColor(TextMeshProUGUI text, Color targetColor, float duration)
    {
        Color startColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            text.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        text.color = targetColor;
    }


    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventHub.PlayerAnimation("Flying", false);
        EventHub.GameOver();
        StartCoroutine(GameOverScreen());
    }

}
