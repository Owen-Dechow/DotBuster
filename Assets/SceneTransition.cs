using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    static SceneTransition _i;

    public static bool Toggled => _i.toggled;
    bool toggled;

    [SerializeField] Image mainPanel;

    private void Awake()
    {
        if (_i)
        {
            Destroy(gameObject);
            return;
        }

        _i = this;
        DontDestroyOnLoad(this);
        toggled = false;
    }

    public static IEnumerator Toggle(bool onOff)
    {
        if (onOff)
            yield return _i.ToggleOn();
        else
            yield return _i.ToggleOff();
    }

    public static void ToggleImmediate(bool onOff)
    {
        Color baseColor = _i.mainPanel.color;
        if (onOff)
        {
            baseColor.a = 1;
            _i.toggled = true;
            _i.gameObject.SetActive(true);
        }
        else
        {
            baseColor.a = 0;
            _i.toggled = false;
            _i.gameObject.SetActive(false);
        }
        _i.mainPanel.color = baseColor;
    }

    IEnumerator ToggleOn()
    {
        gameObject.SetActive(true);

        Color baseColor = mainPanel.color;
        yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
            (float t) => mainPanel.color = new Color(baseColor.r, baseColor.g, baseColor.b, t)
        }, 1);

        toggled = true;
    }

    IEnumerator ToggleOff()
    {
        Color baseColor = mainPanel.color;
        print(Time.time);
        yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
            (float t) => mainPanel.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1 - t),
        }, 1);

        print(Time.time);
        toggled = false;
        gameObject.SetActive(false);
    }
}
