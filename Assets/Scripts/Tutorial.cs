using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private static Tutorial _i;
    private SpriteRenderer spr;
    [SerializeField] Sprite mouseUp;
    [SerializeField] Sprite mouseDown;
    [SerializeField] TMPro.TextMeshProUGUI text;
    public static bool TutorialRunning { get; private set; }

    void Start()
    {
        TutorialRunning = true;

        if (!GameManager.PlayWithTutorial)
        {
            Destroy(gameObject);
        }
        _i = this;

        spr = GetComponent<SpriteRenderer>();
    }

    public static void StartTutorial()
    {
        if (!GameManager.PlayWithTutorial)
        {
            TutorialRunning = false;
            return;
        }

        if (GameManager.Mobile)
        {
            _i.StartCoroutine(_i.MobileTutorial());
        }
        else
        {
            _i.StartCoroutine(_i.PCTutorial());
        }
    }

    IEnumerator MobileTutorial()
    {
        IEnumerator waitForText(string words)
        {
            text.gameObject.SetActive(true);
            text.text = words + "\n" + "Click to continue";
            yield return new WaitForSecondsRealtime(1);
            yield return new WaitUntil(() => Input.touchCount == 0);
            yield return new WaitUntil(() => Input.touchCount > 0);
            text.gameObject.SetActive(false);
        }
        IEnumerator mouseClickAnim()
        {
            while (true)
            {
                text.gameObject.SetActive(true);
                text.text = "Press and hold to activate joystick";
                transform.position = new Vector3(4, 1.5f, 0);
                spr.enabled = true;
                spr.sprite = mouseUp;
                yield return new WaitForSecondsRealtime(0.5f);
                spr.sprite = mouseDown;
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
        IEnumerator waitForClick()
        {
            yield return new WaitUntil(() => Input.touchCount > 0);
        }

        Time.timeScale = 0;
        Coroutine subRoutine = StartCoroutine(mouseClickAnim());

        yield return waitForClick();
        StopCoroutine(subRoutine);
        text.text = "Pay attention to the direction of the joy stick";
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;
        spr.enabled = false;


        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;

        yield return waitForText("Notice the dot bounced in the direction of the joystick");
        yield return waitForText("Collect as many dots as possible to gain points");
        yield return waitForText("Special colored dots are worth extra points but many have strange side effects");
        yield return waitForText("If you bounce out of bounds the round is over");
        Time.timeScale = 1;

        PlayerPrefs.SetInt("HasPlayed", 1);
        TutorialRunning = false;
    }
    IEnumerator PCTutorial()
    {
        IEnumerator waitForText(string words)
        {
            text.gameObject.SetActive(true);
            text.text = words + "\n" + "Click to continue";
            yield return new WaitForSecondsRealtime(1);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            text.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 0;

        IEnumerator mouseAnim()
        {
            text.gameObject.SetActive(true);
            text.text = "Move mouse over screen to control player";
            transform.position = new Vector3(-7, -2, 0);
            spr.enabled = true;
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < 2)
            {
                transform.RotateAround(Vector3.zero, Vector3.forward, (Time.unscaledTime - startTime) / -2);
                transform.rotation = new Quaternion();
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }

        yield return mouseAnim();
        yield return mouseAnim();
        yield return mouseAnim();
        Time.timeScale = 1;
        spr.enabled = false;
        text.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;

        yield return waitForText("Notice the dot bounced in the direction of your mouse");
        yield return waitForText("Collect as many dots as possible to gain points");
        yield return waitForText("Special colored dots are worth extra points but many have strange side effects");
        yield return waitForText("If you bounce out of bounds the round is over");

        Time.timeScale = 1;
        TutorialRunning = false;
        PlayerPrefs.SetInt("HasPlayed", 1);
    }
}
