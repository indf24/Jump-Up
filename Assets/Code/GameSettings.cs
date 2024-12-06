using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ApplyGameSettings();
    }

    private void ApplyGameSettings()
    {
        Application.targetFrameRate = 400;

        QualitySettings.vSyncCount = 0;

        Screen.SetResolution(1080, 1920, true);

        ApplyLetterbox();
    }

    private void ApplyLetterbox()
    {
        // Define the desired aspect ratio
        float targetAspect = 9f / 16f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f) // Add pillarbox (black bars on sides)
        {
            Camera.main.rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
        }
        else // Add letterbox (black bars on top/bottom)
        {
            float scaleWidth = 1.0f / scaleHeight;
            Camera.main.rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
        }
    }
}
