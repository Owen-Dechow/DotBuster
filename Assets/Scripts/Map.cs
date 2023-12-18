using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] float baseWidth;
    [SerializeField] RectTransform player;
    private RectTransform rectTransform;
    private float yRatio;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        //Set map size
        yRatio = BumpersGrid.LevelSize.y / BumpersGrid.LevelSize.x;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseWidth * yRatio);
    }

    private void Update()
    {
        Vector2 position = baseWidth * (CameraController.Position / BumpersGrid.LevelSize.x);
        player.localPosition = position - (new Vector2(baseWidth, -baseWidth * yRatio) / 2f);
    }
}
