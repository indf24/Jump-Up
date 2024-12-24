using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverButtons;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private GameObject currentScore;
    [SerializeField] private GameObject UIBall;

    [SerializeField] private GameObject[] uiElements; // Drag UI elements here in the Inspector
    private List<float> targetPositionsEnter = new() { -375f, -350f, -440f, -425f };
    private List<float> targetPositionsLeave = new() { -715f, -690f, -780f, -765 };
    private List<float> delays = new() { 0.2f, 0.2f, 0.2f, 1f };
    private float moveDuration = 0.5f;

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
        StartCoroutine(UIMove(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000), 0.5f));
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
        rb.velocity = Vector2.zero;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        ball.transform.position = new Vector2(0, -1.25f);
        ball.transform.parent = platform.transform;
    }

    private IEnumerator ShowGameOverUI()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            StartCoroutine(UIMove(uiElements[i], new(targetPositionsEnter[i], uiElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration));
            yield return new WaitForSeconds(delays[i]);
        }

        StartCoroutine(UIMove(UIBall, new(550, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
        StartCoroutine(UIMove(gameOverButtons, new(405, gameOverButtons.GetComponent<RectTransform>().anchoredPosition.y), 0.5f));
    }

    private void HideGameOverUI()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            StartCoroutine(UIMove(uiElements[i], new(targetPositionsLeave[i], uiElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration));
        }

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
