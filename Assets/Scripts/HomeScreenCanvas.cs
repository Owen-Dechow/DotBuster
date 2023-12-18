using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenCanvas : MonoBehaviour
{
    [SerializeField] Slider musicVol;
    [SerializeField] Slider SFXVol;
    [SerializeField] AudioSource aus;
    [SerializeField] Image soundBTN;
    [SerializeField] Sprite soundOn;
    [SerializeField] Sprite soundOff;

    [SerializeField] RectTransform standardCanvas;
    [SerializeField] RectTransform highScoreCanvas;
    [SerializeField] RectTransform creditsCanvas;

    [SerializeField] GameObject playBTN;
    [SerializeField] GameObject exitGameBTN;

    [SerializeField] UnityEngine.EventSystems.EventSystem eventSystem;

    bool cancelTest = true;

    private void Start()
    {
        musicVol.value = GameManager.MusicVol;
        SFXVol.value = GameManager.SFXVol;
        SetSoundIcon();

        if (PlayerPrefs.GetInt("HasPlayed", 0) == 0)
        {
            playBTN.GetComponent<Button>().interactable = false;
            playBTN.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        if (GameManager.Mobile) Destroy(exitGameBTN);
    }

    public void StartGame(bool tutorial)
    {
        GameManager.LoadScene(GameManager.LevelScene, tutorial: tutorial);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMusicVol() => GameManager.MusicVol = musicVol.value;
    public void SetSFXVol()
    {
        aus.volume = SFXVol.value;
        GameManager.SFXVol = SFXVol.value;
        if (!GameManager.SoundOn) return;
        if (cancelTest)
        {
            cancelTest = false;
            return;
        }
        aus.Play();
    }
    public void ToggleSound()
    {
        GameManager.SoundOn = !GameManager.SoundOn;
        SetSoundIcon();
    }
    private void SetSoundIcon()
    {
        if (GameManager.SoundOn) soundBTN.sprite = soundOn;
        else soundBTN.sprite = soundOff;

    }

    public void SwitchToHighScoreCanvas()
    {
        IEnumerator switchTo()
        {
            eventSystem.enabled = false;
            highScoreCanvas.gameObject.SetActive(true);

            RectTransformLerpTarget standard_lt = new RectTransformLerpTarget(standardCanvas);
            RectTransformLerpTarget highScores_lt = new RectTransformLerpTarget(highScoreCanvas);

            highScores_lt.CorrectFinalAnchorPivotScheme();
            standard_lt.SetFinalAnchorPivotScheme(
                new Vector2(1, .5f),
                new Vector2(1, .5f),
                new Vector2(0, .5f)
            );

            yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
                highScores_lt.LerpRectTransformTo,
                standard_lt.LerpRectTransformTo,
            }, 1);

            standardCanvas.gameObject.SetActive(false);
            eventSystem.enabled = true;
        }
        StartCoroutine(switchTo());
    }
    public void SwitchToCreditsCanvas()
    {
        IEnumerator switchTo()
        {
            eventSystem.enabled = false;
            creditsCanvas.gameObject.SetActive(true);

            RectTransformLerpTarget standard_lt = new RectTransformLerpTarget(standardCanvas);
            RectTransformLerpTarget credits_lt = new RectTransformLerpTarget(creditsCanvas);

            credits_lt.CorrectFinalAnchorPivotScheme();
            standard_lt.SetFinalAnchorPivotScheme(
                new Vector2(1, .5f),
                new Vector2(1, .5f),
                new Vector2(0, .5f)
            );

            yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
                credits_lt.LerpRectTransformTo,
                standard_lt.LerpRectTransformTo,
            }, 1);

            standardCanvas.gameObject.SetActive(false);
            eventSystem.enabled = true;
        }
        StartCoroutine(switchTo());
    }
    public void SwitchFromHighScoreCanvas()
    {
        IEnumerator switchTo()
        {
            eventSystem.enabled = false;
            standardCanvas.gameObject.SetActive(true);

            RectTransformLerpTarget standard_lt = new RectTransformLerpTarget(standardCanvas);
            RectTransformLerpTarget highScore_lt = new RectTransformLerpTarget(highScoreCanvas);

            standard_lt.CorrectFinalAnchorPivotScheme();
            highScore_lt.SetFinalAnchorPivotScheme(
                new Vector2(.5f, 0),
                new Vector2(.5f, 0),
                new Vector2(.5f, 1)
            );

            yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
                highScore_lt.LerpRectTransformTo,
                standard_lt.LerpRectTransformTo,
            }, 1);

            highScoreCanvas.gameObject.SetActive(false);
            eventSystem.enabled = true;
        }
        StartCoroutine(switchTo());
    }
    public void SwitchFromCreditsCanvas()
    {
        IEnumerator switchTo()
        {
            eventSystem.enabled = false;
            standardCanvas.gameObject.SetActive(true);

            RectTransformLerpTarget standard_lt = new RectTransformLerpTarget(standardCanvas);
            RectTransformLerpTarget credits_lt = new RectTransformLerpTarget(creditsCanvas);

            standard_lt.CorrectFinalAnchorPivotScheme();
            credits_lt.SetFinalAnchorPivotScheme(
                new Vector2(.5f, 1),
                new Vector2(.5f, 1),
                new Vector2(.5f, 0)
            );

            yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
                credits_lt.LerpRectTransformTo,
                standard_lt.LerpRectTransformTo,
            }, 1);

            creditsCanvas.gameObject.SetActive(false);
            eventSystem.enabled = true;
        }
        StartCoroutine(switchTo());
    }
}
