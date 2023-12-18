using System.Collections;
using UnityEngine;


struct RectTransformLerpTarget
{
    readonly RectTransform rect;
    public Values values;
    Values originalValues;

    public RectTransformLerpTarget(RectTransform rect)
    {
        this.rect = rect;
        values = new Values(rect);
        originalValues = values;
    }

    public void LerpRectTransformTo(float t)
    {
        rect.anchorMax = Vector2.Lerp(originalValues.anchorMax, values.anchorMax, t);
        rect.anchorMin = Vector2.Lerp(originalValues.anchorMin, values.anchorMin, t);
        rect.pivot = Vector2.Lerp(originalValues.pivot, values.pivot, t);
        rect.rotation = Quaternion.Euler(Vector3.Lerp(originalValues.rotation, values.rotation, t));
        rect.anchoredPosition = Vector3.Lerp(originalValues.anchoredPosition, values.anchoredPosition, t);
        rect.localScale = Vector3.Lerp(originalValues.scale, values.scale, t);
    }

    public void CorrectFinalAnchorPivotScheme()
    {
        Vector2 p5 = new(.5f, .5f);
        values.pivot = p5;
        values.anchorMax = p5;
        values.anchorMin = p5;
    }

    public void SetRootFinalAnchoredY(float y)
    {
        originalValues.anchoredPosition.y = y;
        values.anchoredPosition.y = y;
    }

    public void SetRootAnchoredXRandom(float range)
    {
        originalValues.anchoredPosition.x = range * Random.value;
    }

    public void SetXAnchors(float x)
    {
        values.anchorMax.x = x;
        values.anchorMin.x = x;
    }

    internal void SetFinalAnchoredXRandom(float range)
    {
        values.anchoredPosition.x = range * Random.value;
    }

    public static IEnumerator RunOnCurveOverSeconds(System.Action<float>[] actions, float seconds)
    {
        System.Func<float, float> curve = x => x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;

        for (float i = 0; i < seconds; i += Mathf.Min(Time.unscaledDeltaTime, 0.1f))
        {
            float t = curve(i);
            yield return new WaitForEndOfFrame();
            foreach (System.Action<float> action in actions)
            {
                action(t);
            }
        }
    }

    public void SetFinalAnchorPivotScheme(Vector2 anMin, Vector2 anMax, Vector2 pivot)
    {
        values.anchorMin = anMin;
        values.anchorMax = anMax;
        values.pivot = pivot;
    }

    public struct Values
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector3 rotation;
        public Vector3 anchoredPosition;
        public Vector3 scale;

        public Values(RectTransform rect)
        {
            anchorMax = rect.anchorMax;
            anchorMin = rect.anchorMin;
            pivot = rect.pivot;
            rotation = rect.rotation.eulerAngles;
            anchoredPosition = rect.anchoredPosition3D;
            scale = rect.localScale;
        }
    }
}

