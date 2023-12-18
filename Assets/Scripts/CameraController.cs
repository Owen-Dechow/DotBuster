using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Vector2 Position { get => _i.transform.position; }
    public static Vector2 Size { get => GetSize(); }

    private static CameraController _i;
    [SerializeField] Transform target;

    private void Awake()
    {
        _i = this;
    }

    void Update()
    {
        transform.position = target.position;
        transform.Translate(new Vector3(0, 0, -10));
    }

    static Vector2 GetSize()
    {
        float width = Camera.main.orthographicSize * Camera.main.aspect;
        float height = Camera.main.orthographicSize;
        return new Vector2(width, height);
    }
}