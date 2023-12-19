using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _i;

    public static bool PlayWithTutorial { get; set; }

    private bool sound;
    public static bool SoundOn { get => _i.sound; set => _i.ToggleSound(value); }

    private float musicVol;
    private float sfxVol;

    public static float MusicVol { get => _i.musicVol; set => _i.SetMusicVol(value); }
    public static float SFXVol { get => _i.sfxVol; set => _i.SetSFXVol(value); }

    private AudioSource aus;

    [SerializeField] bool mobile;
    public static bool Mobile { get => _i.mobile; }

    public const string LoadGameScene = "LoadGame";
    public const string HomeScreenScene = "HomeScreen";
    public const string LevelScene = "Level";
    private static string scene = LoadGameScene;

    PowerUpTerminationLine powerUpLine;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _i = this;

        aus = GetComponent<AudioSource>();
        aus.Pause();

        SoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        MusicVol = PlayerPrefs.GetFloat("MusicVol", 1);
        SFXVol = PlayerPrefs.GetFloat("SFXVol", 1);

        powerUpLine = new PowerUpTerminationLine();
    }

    void Start()
    {
        SceneTransition.ToggleImmediate(true);
        SceneManager.LoadScene(HomeScreenScene);
        StartCoroutine(SceneTransition.Toggle(false));
    }

    private void Update()
    {
        if (scene == LevelScene)
            powerUpLine.Tick();
    }
    public static void LoadScene(string sceneConst, bool tutorial = false)
    {
        IEnumerator transition()
        {
            yield return SceneTransition.Toggle(true);
            scene = sceneConst;
            PlayWithTutorial = tutorial;
            SceneManager.LoadScene(sceneConst);
            yield return SceneTransition.Toggle(false);
        }

        _i.StartCoroutine(transition());
    }

    void ToggleSound(bool onOff)
    {
        sound = onOff;
        if (onOff)
        {
            aus.UnPause();
            PlayerPrefs.SetInt("SoundOn", 1);
        }
        else
        {
            aus.Pause();
            PlayerPrefs.SetInt("SoundOn", 0);
        }
    }
    private void SetMusicVol(float vol)
    {
        aus.volume = vol;
        musicVol = vol;
        PlayerPrefs.SetFloat("MusicVol", vol);
    }
    private void SetSFXVol(float vol)
    {
        sfxVol = vol;
        PlayerPrefs.SetFloat("SFXVol", vol);
    }

    public static void LoadPower(BumperType.Action actionType, Action action)
    {
        _i.powerUpLine.Add(actionType, action);
    }
    public static bool PowerTypeEnabled(BumperType.Action actionType)
    {
        return _i.powerUpLine.HasActionType(actionType);
    }
    public static float AngleTo(Vector3 from, Vector3 to)
    {

        Vector3 toPos = to;
        toPos.z = 0;

        Vector3 fromPos = from;
        fromPos.z = 0;

        toPos.x -= fromPos.x;
        toPos.y -= fromPos.y;

        float angle = Mathf.Atan2(toPos.y, toPos.x) * Mathf.Rad2Deg;

        return angle;
    }
    public static void DumpPowers()
    {
        _i.powerUpLine = new PowerUpTerminationLine();
    }
    class PowerUpTerminationLine
    {
        readonly List<Member> members;
        readonly List<BumperType.Action> memberActionTypes;
        float lastTick;
        const float PowerUpLife = 15;

        public PowerUpTerminationLine()
        {
            members = new List<Member>();
            memberActionTypes = new List<BumperType.Action>();
            lastTick = Time.time;
        }

        public bool HasActionType(BumperType.Action actionType) => memberActionTypes.Contains(actionType);

        public void Add(BumperType.Action actionType, Action action)
        {
            float time = PowerUpLife;
            if (members.Count > 0)
            {
                Member last = members[^1];
                time -= last.timeFromLast;
            }

            members.Add(new Member(time, action));
            memberActionTypes.Add(actionType);
        }

        public void Tick()
        {
            float timePassed = Time.time - lastTick;
            lastTick = Time.time;

            if (members.Count == 0)
                return;

            Member first = members[0];
            first.timeFromLast -= timePassed;

            if (first.timeFromLast <= 0)
            {
                members.RemoveAt(0);
                memberActionTypes.RemoveAt(0);
                first.action();
            }
        }

        class Member
        {
            public Member(float timeFromLast, Action action)
            {
                this.timeFromLast = timeFromLast;
                this.action = action;
            }

            public float timeFromLast;
            public Action action;
        }
    }
}
