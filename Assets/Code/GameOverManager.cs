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

    [SerializeField] private List<GameObject> gameElements;
    private List<float> targetPositionsEnter = new() { -375f, -350f, -440f, -425f };
    private List<float> targetPositionsLeave = new() { -715f, -690f, -780f, -765 };
    private List<float> delays = new() { 0.2f, 0.2f, 0.2f, 0.6f };
    private float moveDuration = 0.5f;

    [SerializeField] private List<GameObject> menuElements;
    [SerializeField] private TextMeshProUGUI playText;

    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject platform;

    private void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(Retry()));
        menuButton.onClick.AddListener(() => Menu());
    }

    private IEnumerator GameOverScreen()
    {
        StartCoroutine(ResetBall());
        StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000f), 0.5f, "ease", true));
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator Retry()
    {
        HideGameOverUI();
        StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 850f), 0.7f, isCanvasObject:true));
        EventHub.Retry();
        yield return new WaitForSeconds(0.8f);

        SceneControl.ReloadScene();
    }

    private void Menu()
    {
        HideGameOverUI();
        StartCoroutine(LoadMenu());
    }

    private IEnumerator ResetBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector2.zero;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        ball.transform.position = new Vector2(0f, -1.25f);
        ball.transform.parent = platform.transform;
    }

    private IEnumerator ShowGameOverUI()
    {
        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsEnter[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, "ease", true));
            yield return new WaitForSeconds(delays[i]);
        }

        StartCoroutine(Utils.MoveObject(UIBall, new(600f, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, "ease", true));
        StartCoroutine(Utils.MoveObject(gameOverButtons, new(422.5f, gameOverButtons.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, "ease", true));
    }

    private void HideGameOverUI()
    {
        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsLeave[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, "ease", true));
        }

        StartCoroutine(Utils.MoveObject(UIBall, new(950f, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, "ease", true));
        StartCoroutine(Utils.MoveObject(gameOverButtons, new(772.5f, gameOverButtons.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, "ease", true));
    }

    private IEnumerator LoadMenu()
    {
        List<float> targetYPos = new() { 20f, -80f };
        List<float> duration = new() { 0.7f, 0.2f };

        for (int i = 0; i < menuElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(menuElements[i],
                new(i == 1 ? menuElements[i].GetComponent<RectTransform>().anchoredPosition.x : menuElements[i].transform.position.x,
                targetYPos[i]), duration[i],
                isCanvasObject: i == 1));

            if (i == 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        StartCoroutine(Utils.ChangeOpacityOverTime(playText, 1f, 0.2f));

        yield return new WaitForSeconds(0.2f);

        SceneControl.LoadScene("MainMenu");
    }

    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventHub.PlayerAnimation("Flying", false);
        EventHub.GameOver();
        StartCoroutine(GameOverScreen());
    }

}
