using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float bumpForce;
    [SerializeField] float stretch;

    AudioSource aus;
    Rigidbody2D rb;
    ParticleSystem ps;
    SpriteRenderer sr;
    Collider2D col;

    public static float BaseScale { get => _i.baseScale; set => _i.baseScale = value; }
    public static bool Magnet { get => _i.magnet; set => _i.magnet = value; }

    public float baseScale;
    private bool alive;
    static Player _i;
    private bool magnet;

    private void Awake()
    {
        _i = this;
    }

    private void Start()
    {
        baseScale = 1;
        magnet = false;
        Time.timeScale = 1;

        alive = true;
        aus = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        ps = GetComponent<ParticleSystem>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        SpeedStretch();

        // Set point in direction moving
        float rotation = GameManager.AngleTo(transform.position, transform.position + (Vector3)rb.velocity);
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        // Check if in level bounds
        bool validXPosition = Mathf.Abs(transform.position.x) <= BumpersGrid.LevelSize.x / 2 + 1;
        bool validYPosition = Mathf.Abs(transform.position.y) <= BumpersGrid.LevelSize.y / 2 + 1;
        if (!validXPosition || !validYPosition)
        {
            PrivateKillPlayer();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bumper"))
        {
            collision.collider.GetComponent<Bumper>().Hit();
        }
    }

    float GetAngleToMouse()
    {
        float angle = GameManager.AngleTo(Camera.main.WorldToScreenPoint(transform.position), Input.mousePosition);
        return angle;
    }

    public static void ChangeVelocity(bool stop)
    {
        _i.AddForceTowardsMouse(stop);
    }

    void AddForceTowardsMouse(bool stop)
    {
        if (stop) { rb.velocity = Vector2.zero; return; }

        float angle;
        if (GameManager.Mobile)
        {
            angle = MobileInput.Angle;
        }
        else
        {
            angle = GetAngleToMouse();
        }
        Vector2 force = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
        rb.velocity /= 2;
        rb.AddForce(force * bumpForce, ForceMode2D.Impulse);
    }

    void SpeedStretch()
    {
        float thin = baseScale / 3;
        float thick = baseScale + thin;

        float yScale = baseScale - (baseScale * (rb.velocity.magnitude / stretch));
        float xScale = baseScale + (baseScale * (rb.velocity.magnitude / stretch));

        yScale = Mathf.Clamp(yScale, thin, baseScale);
        xScale = Mathf.Clamp(xScale, baseScale, thick);
        transform.localScale = new Vector3(xScale, yScale, 1);
    }

    public static void KillPlayer()
    {
        _i.PrivateKillPlayer();
    }
    private void PrivateKillPlayer()
    {

        if (!alive)
            return;

        rb.bodyType = RigidbodyType2D.Static;
        sr.enabled = false;
        col.enabled = false;

        if (GameManager.SoundOn)
        {
            aus.volume = GameManager.SFXVol;
            aus.Play();
        }

        ps.Play();
        alive = false;
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        Time.timeScale = 1;
        GameManager.DumpPowers();

        while (Time.timeScale > 0)
        {
            Time.timeScale = Mathf.Clamp01(Time.timeScale - Time.unscaledDeltaTime / 2);
            yield return new WaitForEndOfFrame();
        }
        yield return UIController.GameOver();
    }
}
