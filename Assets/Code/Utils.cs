using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
    public enum TransformType
    {
        Normal,
        Ease
    }

    internal static IEnumerator MoveObject(GameObject obj, Vector2 targetPos, float duration, TransformType transformType = TransformType.Normal, bool isCanvasObject = false)
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

    internal static IEnumerator ScaleObject(GameObject obj, Vector2 targetScaleV2, float duration, TransformType transformType = TransformType.Normal, bool isCanvasObject = false)
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

    internal static IEnumerator ChangeOpacityOverTime(Component targetComponent, float targetOpacity, float duration)
    {
        Func<float> getAlpha = null;
        Action<float> setAlpha = null;

        // Determine component type and assign appropriate alpha handlers
        switch (targetComponent)
        {
            case TextMeshProUGUI text:
                getAlpha = () => text.alpha;
                setAlpha = (value) => text.alpha = value;
                break;

            case Image image:
                getAlpha = () => image.color.a;
                setAlpha = (value) =>
                {
                    Color color = image.color;
                    color.a = value;
                    image.color = color;
                };
                break;
        }

        float elapsedTime = 0;
        float startAlpha = getAlpha();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetOpacity, t);
            setAlpha(newAlpha);
            yield return null;
        }

        setAlpha(targetOpacity);
    }

    internal static IEnumerator BlinkText(TextMeshProUGUI text, float blinkTime)
    {
        blinkTime /= 2;

        while (true)
        {
            text.alpha = Mathf.PingPong(Time.time / blinkTime, 1f);
            yield return null;
        }
    }

    internal static Vector2 ConvertPositionToCanvas(Vector2 position) => new(position.x * 64, (position.y * 64) - 960);
}
