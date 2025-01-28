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
        Vector2 startPos;
        RectTransform rectTransform = new();

        if (isCanvasObject)
        {
            rectTransform = obj.GetComponent<RectTransform>();
            startPos = rectTransform.anchoredPosition;
        }
        else
        {
            startPos = obj.transform.position;
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

            if (isCanvasObject)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            }
            else
            {
                obj.transform.position = Vector2.Lerp(startPos, targetPos, t);
            }

            yield return null;
        }

        if (isCanvasObject)
        {
            rectTransform.anchoredPosition = targetPos;
        }
        else
        {
            obj.transform.position = targetPos;
        }
    }

    public static IEnumerator ScaleObject(GameObject obj, Vector2 targetScale, float duration, TransformType transformType = TransformType.Normal, bool isCanvasObject = false)
    {
        Vector2 startScale;
        RectTransform rectTransform = new();

        if (isCanvasObject)
        {
            rectTransform = obj.GetComponent<RectTransform>();
            startScale = rectTransform.localScale;
        }
        else
        {
            startScale = obj.transform.localScale;
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

            if (isCanvasObject)
            {
                rectTransform.localScale = Vector2.Lerp(startScale, targetScale, t);
            }
            else
            {
                obj.transform.localScale = Vector2.Lerp(startScale, targetScale, t);
            }

            yield return null;
        }

        if (isCanvasObject)
        {
            rectTransform.localScale = targetScale;
        }
        else
        {
            obj.transform.localScale = targetScale;
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
}
