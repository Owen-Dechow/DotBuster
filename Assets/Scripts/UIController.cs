using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    [SerializeField] int numberOfMapDots;
    [SerializeField] EventSystem eventSystem;


    [SerializeField] GameObject gameOverCanvas;

    private static UIController _i;
    private int score;

    private void Start()
    {
        _i = this;
        score = 0;
        scoreText.text = "0";
    }

    public static void AddToScore(int addend)
    {
        _i.score += addend;
        _i.scoreText.text = _i.score.ToString();
    }

    public static IEnumerator GameOver()
    {
        bool brokeHighScore = _i.SetUpGameOverCanvas();
        yield return _i.PrivateGameOver(brokeHighScore);
    }

    IEnumerator PrivateGameOver(bool brokeHighScore)
    {
        eventSystem.enabled = false;
        gameOverCanvas.SetActive(true);

        Image gameOverImage = gameOverCanvas.GetComponent<Image>();

        int xOffsetMax = -1000;

        RectTransformLerpTarget[] lts;
        if (brokeHighScore)
        {
            RectTransformLerpTarget highScoreMessage_lt = GetGameOverRectTransformLerpTarget("HighScoreMessage", true);
            RectTransformLerpTarget name_lt = GetGameOverRectTransformLerpTarget("Name", true);
            RectTransformLerpTarget score_lt = GetGameOverRectTransformLerpTarget("Score", true);
            RectTransformLerpTarget continue_lt = GetGameOverRectTransformLerpTarget("Continue", true);
            RectTransformLerpTarget skip_lt = GetGameOverRectTransformLerpTarget("Skip", true);

            highScoreMessage_lt.SetRootFinalAnchoredY(70);
            score_lt.SetRootFinalAnchoredY(32);
            name_lt.SetRootFinalAnchoredY(-16);
            continue_lt.SetRootFinalAnchoredY(-49);
            skip_lt.SetRootFinalAnchoredY(-78);

            highScoreMessage_lt.SetRootAnchoredXRandom(xOffsetMax);
            score_lt.SetRootAnchoredXRandom(xOffsetMax);
            name_lt.SetRootAnchoredXRandom(xOffsetMax);
            continue_lt.SetRootAnchoredXRandom(xOffsetMax);
            skip_lt.SetRootAnchoredXRandom(xOffsetMax);

            lts = new RectTransformLerpTarget[] {
                highScoreMessage_lt,
                score_lt,
                name_lt,
                continue_lt,
                skip_lt,
            };
        }
        else
        {
            RectTransformLerpTarget score_lt = GetGameOverRectTransformLerpTarget("Score", true);
            RectTransformLerpTarget highScore_lt = GetGameOverRectTransformLerpTarget("HighScore", true);
            RectTransformLerpTarget playAgain_lt = GetGameOverRectTransformLerpTarget("PlayAgain", true);
            RectTransformLerpTarget home_lt = GetGameOverRectTransformLerpTarget("Home", true);

            score_lt.SetRootFinalAnchoredY(56);
            highScore_lt.SetRootFinalAnchoredY(22);
            playAgain_lt.SetRootFinalAnchoredY(-5);
            home_lt.SetRootFinalAnchoredY(-39);

            score_lt.SetRootAnchoredXRandom(xOffsetMax);
            highScore_lt.SetRootAnchoredXRandom(xOffsetMax);
            playAgain_lt.SetRootAnchoredXRandom(xOffsetMax);
            home_lt.SetRootAnchoredXRandom(xOffsetMax);

            lts = new RectTransformLerpTarget[] {
                score_lt,
                highScore_lt,
                playAgain_lt,
                home_lt,
            };
        }

        List<System.Action<float>> actions = new();
        foreach (RectTransformLerpTarget lt in lts)
        {
            actions.Add(lt.LerpRectTransformTo);
        }
        actions.Add((float t) => gameOverImage.color = new Color(0, 0, 0, t * 0.8f));
        yield return RectTransformLerpTarget.RunOnCurveOverSeconds(actions.ToArray(), 1);

        eventSystem.enabled = true;
    }

    RectTransformLerpTarget GetGameOverRectTransformLerpTarget(string name, bool centerAnchorsAndPivots)
    {
        RectTransformLerpTarget target = new RectTransformLerpTarget(gameOverCanvas.transform.Find(name) as RectTransform);
        if (centerAnchorsAndPivots)
            target.CorrectFinalAnchorPivotScheme();

        return target;
    }

    IEnumerator SwitchToStandardGameOver()
    {
        eventSystem.enabled = false;

        int maxXOffset = 1000;

        RectTransformLerpTarget name_lt = GetGameOverRectTransformLerpTarget("Name", false);
        RectTransformLerpTarget continue_lt = GetGameOverRectTransformLerpTarget("Continue", false);
        RectTransformLerpTarget skip_lt = GetGameOverRectTransformLerpTarget("Skip", false);

        name_lt.SetXAnchors(1);
        continue_lt.SetXAnchors(1);
        skip_lt.SetXAnchors(1);

        name_lt.values.pivot.x = -0.1f;
        continue_lt.values.pivot.x = -0.1f;
        skip_lt.values.pivot.x = -0.1f;

        name_lt.SetFinalAnchoredXRandom(maxXOffset);
        continue_lt.SetFinalAnchoredXRandom(maxXOffset);
        skip_lt.SetFinalAnchoredXRandom(maxXOffset);

        RectTransformLerpTarget playAgain_lt = GetGameOverRectTransformLerpTarget("PlayAgain", true);
        RectTransformLerpTarget home_lt = GetGameOverRectTransformLerpTarget("Home", true);
        
        playAgain_lt.SetRootFinalAnchoredY(-20);
        home_lt.SetRootFinalAnchoredY(-53);

        playAgain_lt.SetRootAnchoredXRandom(-maxXOffset);
        home_lt.SetRootAnchoredXRandom(-maxXOffset);

        yield return RectTransformLerpTarget.RunOnCurveOverSeconds(new System.Action<float>[] {
            name_lt.LerpRectTransformTo,
            continue_lt.LerpRectTransformTo,
            skip_lt.LerpRectTransformTo,
            home_lt.LerpRectTransformTo,
            playAgain_lt.LerpRectTransformTo,
        }, 1);

        eventSystem.enabled = true;
    }

    private bool SetUpGameOverCanvas()
    {
        int rank = HighScores.GetRanked(score);

        gameOverCanvas.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score;
        gameOverCanvas.transform.Find("HighScore").GetComponent<TMPro.TextMeshProUGUI>().text = "HighScore: " + HighScores.GetHighScore();
        gameOverCanvas.transform.Find("Rank").GetComponent<TMPro.TextMeshProUGUI>().text = "Rank #: " + rank;

        return rank != 0;
    }

    public void MainMenu()
    {
        GameManager.LoadScene(GameManager.HomeScreenScene);
    }
    public void PlayAgain()
    {
        GameManager.LoadScene(GameManager.LevelScene);
    }
    public void SubmitHighScore()
    {
        TMPro.TMP_InputField inputField = gameOverCanvas.transform.Find("Name").GetComponent<TMPro.TMP_InputField>();
        string text = inputField.text.Trim();
        if (text.Length > 0)
        {
            eventSystem.enabled = false;
            HighScores.SubmitHighScore(score, inputField.text);
            StartCoroutine(SwitchToStandardGameOver());
        }
    }
    public void SkipHighScoreSave()
    {
        StartCoroutine(SwitchToStandardGameOver());
    }
    public void SubmitHighScoreFromInput()
    {
        if (GameManager.Mobile) return;
        SubmitHighScore();
    }
    public void ClampHighScoreName()
    {
        TMPro.TMP_InputField inputField = gameOverCanvas.transform.Find("Name").GetComponent<TMPro.TMP_InputField>();
        if (inputField.text.Length < 10) return;
        inputField.text = inputField.text[..10];
    }

}
