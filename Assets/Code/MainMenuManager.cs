using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private List<GameObject> gameObjects;

    private Coroutine blinkingPlayText;
    private bool start = true;

    private Vector2 startTouchPosition;

    private void Start()
    {
        blinkingPlayText = StartCoroutine(Utils.BlinkText(playText, 1f));
        PlayerManager.EnableInput();
    }

    void Update()
    {
        if (Input.touchCount == 0 || !PlayerManager.PlayerInputAllowed)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:
                Vector2 endTouchPosition = touch.position;

                if (Vector2.Distance(startTouchPosition, endTouchPosition) > 30f && start)
                {
                    start = false;
                }

                break;

            case TouchPhase.Ended:
                bool buttonTap = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

                if (buttonTap || !start)
                {
                    start = true;
                    return;
                }

                PlayerManager.DisableInput();
                StartCoroutine(LoadGame());

                break;
        }
    }

    private IEnumerator LoadGame()
    {
        StopCoroutine(blinkingPlayText);
        StartCoroutine(Utils.ChangeOpacityOverTime(playText, 0f, 0.5f));

        List<float> targetYPos = new() { 35f, 50f };
        List<float> duration = new() { 0.7f, 0.2f };

        for (int i = 0; i < gameObjects.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameObjects[i],
                new(i == 1 ? gameObjects[i].GetComponent<RectTransform>().anchoredPosition.x : gameObjects[i].transform.position.x, targetYPos[i]),
                duration[i],
                isCanvasObject: i == 1));
        }

        yield return new WaitForSeconds(0.5f);
        GameCoordinator.instance.PrepareGameMode1();
        yield return new WaitForSeconds(0.7f);

        SceneControl.LoadScene("GameMode1");
    }
}
