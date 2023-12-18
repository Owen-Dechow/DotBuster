using System.Collections.Generic;
using UnityEngine;

public class BumpersGrid : MonoBehaviour
{
    static BumpersGrid _i;

    [SerializeField] GameObject bumperPrefab;

    [SerializeField] BumperType _normalBumper;
    [SerializeField] BumperType[] _otherBumpers;

    [SerializeField] byte normalOtherRatio;

    [SerializeField] GameObject bumpersGridTilePrefab;
    [SerializeField] Vector2Int gridDimensions;
    [SerializeField] float tileSize;
    [SerializeField] int numberOfBumpers;

    List<BumpersGridTile> bumpersGridTiles;
    List<BumpersGridTile> renderedTiles;

    Vector2 levelSize;
    public static Vector2 LevelSize => _i.levelSize;

    private void Awake()
    {
        _i = this;
        levelSize = (Vector2)gridDimensions * tileSize;
    }

    void Start()
    {
        bumpersGridTiles = new List<BumpersGridTile>();
        renderedTiles = new List<BumpersGridTile>();

        // Create grid tiles
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                GameObject tileGO = Instantiate(bumpersGridTilePrefab);
                BumpersGridTile tile = tileGO.GetComponent<BumpersGridTile>();

                tile.transform.SetParent(transform);
                tile.index = new Vector2(x, y);
                tile.PlaceTileBasedOnIndex(gridDimensions, tileSize);

                bumpersGridTiles.Add(tile);
            }
        }


        // Create bumpers
        for (int _ = 0; _ < numberOfBumpers; _++)
        {
            Vector2 position = Vector2.zero;
            while (Vector2.Distance(position, Vector2.zero) < 6)
            {
                position = new(
                   Random.Range(levelSize.x / -2, levelSize.x / 2),
                   Random.Range(levelSize.y / -2, levelSize.y / 2)
                   );
            }

            BumperType type;
            if (Random.Range(0, normalOtherRatio) == 0)
                type = _otherBumpers[Random.Range(0, _otherBumpers.Length)];
            else
                type = _normalBumper;

            GameObject bumperGO = Instantiate(bumperPrefab);
            PrivateGetTileFromLocation(position).Add(bumperGO, position, type);
        }

        // Add Constant Bumper
        Vector2 constantBumperPosition = Vector2.down * 3;
        GameObject constantBumperGO = Instantiate(bumperPrefab);
        PrivateGetTileFromLocation(constantBumperPosition).Add(constantBumperGO, constantBumperPosition, _normalBumper);

        Spawning();
    }

    private void LateUpdate()
    {
        Spawning();
    }

    public static BumpersGridTile GetTileFromLocation(Vector2 location)
    {
        return _i.PrivateGetTileFromLocation(location);
    }
    BumpersGridTile PrivateGetTileFromLocation(Vector2 location)
    {
        int x = Mathf.FloorToInt((location.x + levelSize.x / 2) / tileSize);
        int y = Mathf.FloorToInt((location.y + levelSize.y / 2) / tileSize);
        int idx = x * gridDimensions.y + y;

        if (idx < 0 || idx > bumpersGridTiles.Count)
            return null;

        return bumpersGridTiles[idx];
    }

    void Spawning()
    {
        Camera camera = Camera.main;
        Vector3 position = camera.transform.position;
        float camHeight = camera.orthographicSize + tileSize;
        float camWidth = camera.orthographicSize * camera.aspect + tileSize;

        List<BumpersGridTile> renderedTiles = new List<BumpersGridTile>();
        for (float x = position.x - camWidth; x <= position.x + camWidth; x += tileSize)
        {
            for (float y = position.y - camHeight; y <= position.y + camHeight; y += tileSize)
            {
                BumpersGridTile tile = PrivateGetTileFromLocation(new Vector2(x, y));

                if (tile != null)
                {
                    renderedTiles.Add(tile);

                    if (!this.renderedTiles.Contains(tile))
                        tile.gameObject.SetActive(true);
                }
            }
        }

        this.renderedTiles.ForEach((x) =>
        {
            if (!renderedTiles.Contains(x))
                x.gameObject.SetActive(false);
        });
        this.renderedTiles = renderedTiles;
    }
}


