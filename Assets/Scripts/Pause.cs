using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject OpenCloseBTN;
    [SerializeField] GameObject contentPanel;
    [SerializeField] Sprite soundOn;
    [SerializeField] Sprite soundOff;
    [SerializeField] Image soundImage;
    [SerializeField] GameObject clickMessage;

    bool opened;
    float initialTimeScale;
    Coroutine coroutine;

    private void Start()
    {
        opened = false;
        contentPanel.transform.localScale = new Vector3(1, 0, 1);

        if (GameManager.PlayWithTutorial && GameManager.Mobile)
        {
            Destroy(clickMessage);
            Tutorial.StartTutorial();
        }
        else
        {

            IEnumerator waitForClick()
            {

                initialTimeScale = Time.timeScale;
                {
                    Time.timeScale = 0;
                    yield return new WaitWhile(() => SceneTransition.Toggled);

                    if (GameManager.Mobile)
                        yield return new WaitUntil(() => Input.touchCount > 0);
                    else
                        yield return new WaitUntil(() => Input.GetMouseButton(0));

                    Destroy(clickMessage);
                    Tutorial.StartTutorial();
                }

                Time.timeScale = initialTimeScale;
            }
            StartCoroutine(waitForClick());
        }
    }

    public void ToggleOnOff()
    {
        if (Tutorial.TutorialRunning) return;
        opened = !opened;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ToggleAnimation(opened));
    }

    public void ToggleSound()
    {
        GameManager.SoundOn = !GameManager.SoundOn;
        SetSoundIcon();
    }

    private IEnumerator ToggleAnimation(bool on)
    {
        if (on)
        {
            initialTimeScale = Time.timeScale;
            Time.timeScale = 0;
            SetSoundIcon();
            while (contentPanel.transform.localScale.y < 0.999f)
            {
                yield return new WaitForEndOfFrame();
                contentPanel.transform.localScale += 4 * Time.unscaledDeltaTime * (Vector3.one - contentPanel.transform.localScale);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (4 * Time.unscaledDeltaTime * (new Vector3(0, 0, 360) - transform.rotation.eulerAngles)));
            }
        }
        else
        {
            Time.timeScale = initialTimeScale;
            while (contentPanel.transform.localScale.y > 0.001f)
            {
                yield return new WaitForEndOfFrame();
                contentPanel.transform.localScale += 4 * Time.unscaledDeltaTime * (new Vector3(1, 0, 1) - contentPanel.transform.localScale);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (4 * Time.unscaledDeltaTime * (new Vector3(0, 0, 0) - transform.rotation.eulerAngles)));

            }
        }
    }

    private void SetSoundIcon()
    {
        if (GameManager.SoundOn) soundImage.sprite = soundOn;
        else soundImage.sprite = soundOff;

    }
}
