using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject platform;

    [SerializeField] private GameObject continueText;
    [SerializeField] private Button continueButton;

    private bool firstTry = true;
    private bool secondChanceReady = false;

    private void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(Retry()));
        menuButton.onClick.AddListener(() => Menu());
        //continueButton.onClick.AddListener(() => EventHub.Continue());

        EventHub.OnSecondChance += StartSecondChance;
    }

    private void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0 && secondChanceReady)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                
                if (hit.collider == null)
                {
                    StartCoroutine(NoSecondChance());
                }
                else
                {
                    EventHub.SecondChance();
                }
            }
        }
    }

    private IEnumerator ContinueScreen()
    {
        FreezeBall();
        ball.transform.position = new Vector2(0, -1);

        continueText.SetActive(true);
        continueButton.gameObject.SetActive(true);

        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000f), 0.5f, Utils.TransformType.Ease, true));
        yield return StartCoroutine(Utils.MoveObject(ball, new Vector2(0, 15), 0.8f, Utils.TransformType.Ease));
        yield return StartCoroutine(Utils.ScaleObject(ball, new Vector2(15, 15), 0.8f, Utils.TransformType.Ease));

        secondChanceReady = true;
    }

    private void StartSecondChance() => StartCoroutine(SecondChance());

    private IEnumerator SecondChance()
    {
        secondChanceReady = false;

        continueText.SetActive(false);
        continueButton.gameObject.SetActive(false);

        yield return StartCoroutine(Utils.ScaleObject(ball, new Vector2(1, 1), 0.8f, Utils.TransformType.Ease));
        UnfreezeBall();
        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 850), 0.7f, isCanvasObject: true));

        PlayerManager.EnableInput();
    }

    private IEnumerator NoSecondChance()
    {
        secondChanceReady = false;

        continueText.SetActive(false);
        continueButton.gameObject.SetActive(false);

        yield return StartCoroutine(Utils.ScaleObject(ball, new Vector2(1, 1), 0.8f, Utils.TransformType.Ease));
        UnfreezeBall();
        platform.SetActive(false);
    }

    private IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(0.5f);
        ResetBall();
        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000f), 0.5f, Utils.TransformType.Ease, true));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator Retry()
    {
        HideGameOverUI();
        StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 850f), 0.7f, isCanvasObject: true));
        EventHub.Retry();
        yield return new WaitForSeconds(0.8f);

        SceneControl.ReloadScene();
    }

    private void Menu()
    {
        HideGameOverUI();
        StartCoroutine(LoadMenu());
    }

    private void ResetBall()
    {
        FreezeBall();
        ball.transform.position = new Vector2(0f, -1.25f);
        ball.transform.parent = platform.transform;
    }

    private void FreezeBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.rotation = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void UnfreezeBall()
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
    }

    private IEnumerator ShowGameOverUI()
    {
        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsEnter[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, Utils.TransformType.Ease, true));
            yield return new WaitForSeconds(delays[i]);
        }

        StartCoroutine(Utils.MoveObject(UIBall, new(600f, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, Utils.TransformType.Ease, true));
    }

    private void HideGameOverUI()
    {
        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsLeave[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, Utils.TransformType.Ease, true));
        }

        StartCoroutine(Utils.MoveObject(UIBall, new(950f, UIBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, Utils.TransformType.Ease, true));
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

        yield return new WaitForSeconds(0.2f);

        SceneControl.LoadScene("MainMenu");
    }

    // Shows the game over screen on game over
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager.DisableInput();
        EventHub.PlayerAnimation("Flying", false);
        EventHub.GameOver();

        if (firstTry)
        {
            StartCoroutine(ContinueScreen());
            firstTry = false;
        }
        else
        {
            StartCoroutine(GameOverScreen());
        }
    }

    private void OnDestroy() => EventHub.OnSecondChance -= StartSecondChance;
}
