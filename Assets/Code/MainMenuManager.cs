using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
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
                if (!hitUI)
                {
                    SceneControl.LoadScene("GameMode1");
                }
            }
        }
    }
}
