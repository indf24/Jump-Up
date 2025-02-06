using System.Collections;
using TMPro;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public enum TransformType
    {
        Normal,
        Ease
    }

    public static IEnumerator MoveObject(GameObject obj, Vector2 targetPos, float duration, TransformType transformType = TransformType.Normal, bool isCanvasObject = false)
    {
        Vector2 startPos = new();
        RectTransform rectTransform = new();

        switch (isCanvasObject)
        {
            case true:
                rectTransform = obj.GetComponent<RectTransform>();
                startPos = rectTransform.anchoredPosition;
                break;

            case false:
                startPos = obj.transform.position;
                break;
        };

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            if (transformType == TransformType.Ease)
            {
                t = 1 - Mathf.Pow(1 - t, 3);
            }

            switch (isCanvasObject)
            {
                case true:
                    rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                    break;

                case false:
                    obj.transform.position = Vector2.Lerp(startPos, targetPos, t);
                    break;
            }

            yield return null;
        }

        switch (isCanvasObject)
        {
            case true:
                rectTransform.anchoredPosition = targetPos;
                break;
            
            case false:
                obj.transform.position = targetPos;
                break;
        }
    }

    public static IEnumerator ScaleObject(GameObject obj, Vector2 targetScaleV2, float duration, TransformType transformType = TransformType.Normal, bool isCanvasObject = false)
    {
        Vector3 startScale;
        Vector3 targetScale = new(targetScaleV2.x, targetScaleV2.y, 1f);
        RectTransform rectTransform = new();

        switch (isCanvasObject)
        {
            case true:
                rectTransform = obj.GetComponent<RectTransform>();
                startScale = rectTransform.localScale;
                break;
            
            case false:
                startScale = obj.transform.localScale;
                break;
        }

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            if (transformType == TransformType.Ease)
            {
                t = 1 - Mathf.Pow(1 - t, 3);
            }

            switch (isCanvasObject)
            {
                case true:
                    rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                    break;

                case false:
                    obj.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                    break;
            }

            yield return null;
        }

        switch (isCanvasObject)
        {
            case true:
                rectTransform.localScale = targetScale;
                break;

            case false:
                obj.transform.localScale = targetScale;
                break;
        }
    }

    public static IEnumerator ChangeOpacityOverTime(TextMeshProUGUI text, float targetOpacity, float duration)
    {
        float elapsedTime = 0;
        duration *= 5;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            text.alpha = Mathf.Lerp(text.alpha, targetOpacity, t);

            yield return null;
        }

        text.alpha = targetOpacity;
    }

    public static IEnumerator BlinkText(TextMeshProUGUI text, float blinkTime)
    {
        blinkTime /= 2;

        while (true)
        {
            text.alpha = Mathf.PingPong(Time.time / blinkTime, 1f);
            yield return null;
        }
    }

    public static Vector2 ConvertPositionToCanvas(Vector2 position) => new(position.x * 64, (position.y * 64) - 960);
}
