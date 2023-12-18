using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] Transform mask;
    [SerializeField] int borderWidth;

    //Start is called before the first frame update
    void Start()
    {
        Vector2 outlineSize = BumpersGrid.LevelSize + Vector2.one;
        transform.localScale = outlineSize;
        mask.transform.localScale = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(outlineSize) - new Vector3(borderWidth, borderWidth, borderWidth));
    }
}
