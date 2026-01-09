using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float tileScale = 0.099f;
    public float spacing = 1.0f;

    private Tile[,] grid;

    private void Awake()
    {
        GenerateGrid();
    }

    #region Grid Generation

    /// <summary>
    /// グリッド全体を生成する
    /// </summary>
    public void GenerateGrid()
    {
        // すでに存在する場合は削除
        if (grid != null)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tileObj.name = $"Tile_{x}_{y}";
                tileObj.transform.localScale = new Vector3(tileScale, 1f, tileScale);

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null)
                    tile = tileObj.AddComponent<Tile>();

                tile.gridPosition = new Vector2Int(x, y);
                grid[x, y] = tile;

            }
        }

        Debug.Log($"Grid生成完了: {width}x{height}");
    }

    #endregion

    #region Grid Access

    public Tile[,] GetAllTiles()
    {
        return grid;
    }

    /// <summary>
    /// 指定した座標のタイルを返す（範囲外ならnull）
    /// </summary>
    public Tile GetTileAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            return null;
        return grid[pos.x, pos.y];
    }

    /// <summary>
    /// ワールド座標をグリッド座標に変換
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / spacing);
        int y = Mathf.RoundToInt(worldPosition.z / spacing);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// グリッド座標をワールド座標に変換
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * spacing, 0, gridPosition.y * spacing);
    }

    /// <summary>
    /// 指定位置が歩行可能かチェック
    /// </summary>
    public bool IsWalkable(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile == null) return false;
        return tile.Walkable;
    }

    #endregion

    #region Tile Color Management

    /// <summary>
    /// タイルの色を取得
    /// </summary>
    public Color GetTileColor(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile == null) return Color.white;

        Renderer renderer = tile.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            return renderer.material.color;
        }

        return Color.white;
    }

    /// <summary>
    /// タイルの色を設定
    /// </summary>
    public void SetTileColor(Vector2Int gridPosition, Color color)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile == null) return;

        Renderer renderer = tile.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = color;
        }
    }

    /// <summary>
    /// すべてのタイルの色をリセット
    /// </summary>
    public void ResetAllTileColors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetTileColor(new Vector2Int(x, y), Color.white);
            }
        }
    }

    #endregion

    #region Utility

    /// <summary>
    /// グリッド範囲内かチェック
    /// </summary>
    public bool IsInBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width &&
               gridPosition.y >= 0 && gridPosition.y < height;
    }

    #endregion
}