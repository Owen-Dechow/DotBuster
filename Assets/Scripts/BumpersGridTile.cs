using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpersGridTile : MonoBehaviour
{
    public Vector2 index;
    List<Bumper> bumpers;

    public void PlaceTileBasedOnIndex(Vector2Int gridDimensions, float tileSize)
    {
        Vector2 centerIndex = (Vector2)gridDimensions / 2;
        Vector2 locationIdx = index - centerIndex;
        Vector2 location = locationIdx * tileSize;

        transform.position = location;
    }

    public void Add(GameObject bumperGO, Vector2 position, BumperType type)
    {
        bumpers ??= new List<Bumper>();

        Bumper bumper = bumperGO.GetComponent<Bumper>();
        bumper.transform.position = position;
        bumper.transform.parent = transform;
        bumper.SetType(type);
        bumpers.Add(bumper);
    }

    public void AddManual(Bumper bumper)
    {
        bumpers.Add(bumper);
        bumper.transform.parent = transform;
    }

    public void Remove(Bumper bumper)
    {
        bumpers.Remove(bumper);
    }
}
