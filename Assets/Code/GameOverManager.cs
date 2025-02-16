using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    internal static GameOverManager instance;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private GameObject currentScore;
    [SerializeField] private GameObject menuBall;

    [SerializeField] private List<GameObject> gameElements;
    private List<float> targetPositionsEnter = new() { -375f, -350f, -440f, -425f };
    private List<float> targetPositionsLeave = new() { -715f, -690f, -780f, -765 };
    private List<float> delays = new() { 0.2f, 0.2f, 0.2f, 0.6f };
    private float moveDuration = 0.5f;

    [SerializeField] private List<GameObject> menuElements;

    [SerializeField] private GameObject platform;

    [SerializeField] private GameObject continueBall;
    [SerializeField] private Button continueButton;
    [SerializeField] private Image continueClose;

    private bool firstTry = true;
    private bool secondChanceReady = false;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        retryButton.onClick.AddListener(() => StartCoroutine(Retry()));
        menuButton.onClick.AddListener(() => Menu());

        continueButton.onClick.AddListener(() =>
#if UNITY_EDITOR
        {
            GameCoordinator.instance.SecondChance();
        });
#else
        LevelPlayManager.instance.ShowRewardedAd());
#endif
    }

    private void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount == 0 || !secondChanceReady)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        bool UITapped = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

        if (UITapped)
            return;

        StartCoroutine(NoSecondChance());
    }

    private IEnumerator ContinueScreen()
    {
        GameObject player = PlayerManager.instance.GetPlayerObject();

        PlayerManager.instance.FreezePlayer();

        continueBall.SetActive(true);
        continueBall.GetComponent<RectTransform>().anchoredPosition = Utils.ConvertPositionToCanvas(player.transform.position);
        player.transform.position = new(0f, -1.25f);
        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000f), 0.5f, Utils.TransformType.Ease, true));
        yield return StartCoroutine(Utils.MoveObject(continueBall, new(0, 0), 0.8f, Utils.TransformType.Ease, true));
        yield return StartCoroutine(Utils.ScaleObject(continueBall, new(15, 15), 0.8f, Utils.TransformType.Ease, true));

        StartCoroutine(Utils.ChangeOpacityOverTime(continueClose, 0.3f, 0.5f));

        secondChanceReady = true;
        continueButton.interactable = true;
    }

    internal void StartSecondChance() => StartCoroutine(SecondChance());

    private IEnumerator SecondChance()
    {
        secondChanceReady = false;
        continueButton.interactable = false;

        for (int i = 0; i < continueBall.transform.childCount; i++)
        {
            continueBall.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Utils.ScaleObject(continueBall, new(1, 1), 0.8f, Utils.TransformType.Ease, true));
        PlayerManager.instance.GetPlayerObject().transform.position = new(0, 15);
        continueBall.SetActive(false);
        PlayerManager.instance.UnfreezePlayer();
        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 850), 0.7f, isCanvasObject: true));

        PlayerManager.EnableInput();
    }

    private IEnumerator NoSecondChance()
    {
        secondChanceReady = false;
        continueButton.interactable = false;

        for (int i = 0; i < continueBall.transform.childCount; i++)
        {
            continueBall.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return StartCoroutine(Utils.ScaleObject(continueBall, new(1, 1), 0.8f, Utils.TransformType.Ease, true));
        PlayerManager.instance.GetPlayerObject().transform.position = new(0, 15);
        continueBall.SetActive(false);
        PlayerManager.instance.UnfreezePlayer();
        platform.SetActive(false);
    }

    private IEnumerator GameOverScreen()
    {
        PlayerManager.instance.ResetPlayer();
        yield return StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 1000f), 0.5f, Utils.TransformType.Ease, true));
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator Retry()
    {
        HideGameOverUI();
        StartCoroutine(Utils.MoveObject(currentScore, new(currentScore.GetComponent<RectTransform>().anchoredPosition.x, 850f), 0.7f, isCanvasObject: true));

        GameCoordinator.instance.Retry();

        yield return new WaitForSeconds(0.8f);

        SceneControl.ReloadScene();
    }

    private void Menu()
    {
        HideGameOverUI();
        StartCoroutine(LoadMenu());
    }

    private IEnumerator ShowGameOverUI()
    {
        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsEnter[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, Utils.TransformType.Ease, true));
            yield return new WaitForSeconds(delays[i]);
        }

        yield return StartCoroutine(Utils.MoveObject(menuBall, new(600f, menuBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, Utils.TransformType.Ease, true));

        retryButton.interactable = true;
        menuButton.interactable = true;
    }

    private void HideGameOverUI()
    {
        retryButton.interactable = false;
        menuButton.interactable = false;

        for (int i = 0; i < gameElements.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameElements[i], new(targetPositionsLeave[i], gameElements[i].GetComponent<RectTransform>().anchoredPosition.y), moveDuration, Utils.TransformType.Ease, true));
        }

        StartCoroutine(Utils.MoveObject(menuBall, new(950f, menuBall.GetComponent<RectTransform>().anchoredPosition.y), 0.5f, Utils.TransformType.Ease, true));
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
        GameCoordinator.instance.GameOver();

        if (firstTry && LevelPlayManager.instance.IsAdReady())
        {
            StartCoroutine(ContinueScreen());
            firstTry = false;
        }
        else
        {
            StartCoroutine(GameOverScreen());
        }
    }
}
