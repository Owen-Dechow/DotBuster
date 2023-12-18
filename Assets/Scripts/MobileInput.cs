using UnityEngine;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour
{
    public static float Angle { get => _i.angle; }
    private static MobileInput _i;
    private Vector3 initialClickLocation;
    private bool clicking;
    private float angle;
    private Image img;

    [SerializeField] GameObject stick;
    [SerializeField] float maxJoystickDistance;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Mobile)
        {
            Destroy(gameObject);
            return;
        }

        _i = this;
        img = GetComponent<Image>();
        clicking = false;
        angle = 90;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (!clicking)
            {
                ToggleSelf(true);
                initialClickLocation = Input.mousePosition;
            }

            PositionJoyStick();
        }
        else
        {
            if (clicking)
            {
                ToggleSelf(false);
            }
        }
    }

    void PositionJoyStick()
    {
        angle = GameManager.AngleTo(initialClickLocation, Input.mousePosition);

        Vector3 position = Input.mousePosition;
        position.z = 0;

        if (Vector3.Distance(initialClickLocation, position) > maxJoystickDistance)
        {
            Vector3 diff = position - initialClickLocation;
            position = initialClickLocation + diff.normalized * maxJoystickDistance;
        }

        transform.position = initialClickLocation;
        stick.transform.position = position;
    }

    void ToggleSelf(bool toggle)
    {
        img.enabled = toggle;
        stick.SetActive(toggle);
        clicking = toggle;
    }
}
