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

    [SerializeField] private GameObject currentScore;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject finalScore;
    [SerializeField] private GameObject bestText;
    [SerializeField] private GameObject bestScore;
    [SerializeField] private GameObject UIBall;

    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject platform;

    private void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(Retry()));
        menuButton.onClick.AddListener(() => SceneControl.LoadScene("MainMenu"));
    }

    private IEnumerator GameOverScreen()
    {
        StartCoroutine(ResetBall());
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator Retry()
    {
        HideGameOverUI();
        EventHub.Retry();
        yield return new WaitForSeconds(0.7f);

        SceneControl.ReloadScene();
    }

    private IEnumerator ResetBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector3.zero;
        ball.transform.position = new Vector2(0, -1.25f);
        ball.transform.parent = platform.transform;
    }

    private IEnumerator ShowGameOverUI()
    {
        currentScore.SetActive(false);

        StartCoroutine(UIMove(scoreText, new(-375f, scoreText.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(UIMove(finalScore, new(-350f, finalScore.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(UIMove(bestText, new(-440f, bestText.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(UIMove(bestScore, new(-425f, bestScore.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(UIMove(UIBall, new(550, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        StartCoroutine(UIMove(gameOverButtons, new(405, gameOverButtons.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
    }

    private void HideGameOverUI()
    {
        scoreText.SetActive(false);
        finalScore.SetActive(false);
        bestText.SetActive(false);
        bestScore.SetActive(false);
        StartCoroutine(UIMove(UIBall, new(950, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        StartCoroutine(UIMove(gameOverButtons, new(805, gameOverButtons.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
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

    private IEnumerator UIMove(GameObject obj, Vector2 targetPos, float duration)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
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
