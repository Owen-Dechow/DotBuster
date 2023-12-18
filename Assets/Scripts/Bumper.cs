using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    BumperType type;
    BumpersGridTile parentTile;
    [SerializeField] float magSpeed;

    public void SetType(BumperType type)
    {
        this.type = type;

        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        spr.color = type.color;
        spr.sprite = type.sprite;
        transform.localScale += Vector3.one * type.scaleAdjuster;

    }

    public void SetParent()
    {
        BumpersGridTile tile = BumpersGrid.GetTileFromLocation(transform.position);
        if (tile != parentTile)
        {
            if (parentTile)
                parentTile.Remove(this);

            parentTile = tile;
            tile.AddManual(this);
        }
    }

    public void Hit()
    {
        Destroy(gameObject);
        type.Execute(transform.position);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (Player.Magnet && type.magnetic)
        {
            float angle = GameManager.AngleTo(transform.position, collision.transform.position);
            Vector2 delta = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
            transform.position += (Vector3)(magSpeed * Time.deltaTime * delta);
            SetParent();
        }
    }
}
