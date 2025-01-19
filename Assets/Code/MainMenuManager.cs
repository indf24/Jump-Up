using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    }

    void Update()
    {
        if (Input.touchCount > 0 && PlayerManager.PlayerInputAllowed)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase is TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase is not TouchPhase.Ended)
            {
                Vector2 endTouchPosition = touch.position;

                if (Vector2.Distance(startTouchPosition, endTouchPosition) > 30f && start)
                {
                    start = false;
                }
            }

            if (touch.phase is TouchPhase.Ended)
            {
                // Get the touch position in screen coordinates
                Vector2 touchPosition = touch.position;

                // Create a PointerEventData to check for UI elements
                PointerEventData eventData = new(EventSystem.current)
                {
                    position = touchPosition
                };

                List<RaycastResult> results = new();
                EventSystem.current.RaycastAll(eventData, results);

                // Check if any UI element was hit
                bool hitUI = false;
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        hitUI = true;
                        break;
                    }
                }

                // If no UI element was hit, handle the click
                if (!hitUI && start)
                {
                    PlayerManager.DisableInput();
                    StartCoroutine(LoadGame());
                }
                else
                {
                    start = true;
                }
            }
        }
    }

    private IEnumerator LoadGame()
    {
        StopCoroutine(blinkingPlayText);
        StartCoroutine(Utils.ChangeOpacityOverTime(playText, 0f, 0.5f));

        List<float> targetYPos = new() { 35f, 50f, 5.75f };
        List<float> duration = new() { 0.7f, 0.2f, 0.7f };

        for (int i = 0; i < gameObjects.Count; i++)
        {
            StartCoroutine(Utils.MoveObject(gameObjects[i], 
                new(i == 1 ? gameObjects[i].GetComponent<RectTransform>().anchoredPosition.x : gameObjects[i].transform.position.x, 
                targetYPos[i]), duration[i], 
                isCanvasObject: i == 1));

            if (i == 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(0.7f);

        SceneControl.LoadScene("GameMode1");
    }
}
