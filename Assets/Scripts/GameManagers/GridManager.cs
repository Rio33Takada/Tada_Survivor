using UnityEngine;

public class GridManager : MonoBehaviour
{
    private const string LOG_PREFIX = "[GridManager]";

    [Header("Grid Settings")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileScale = 0.099f;
    [SerializeField] private float spacing = 1.0f;

    private Tile[,] grid;

    public int Width => width;
    public int Height => height;
    public float Spacing => spacing;

    #region Unity Lifecycle

    private void Awake()
    {
        GenerateGrid();
    }

    #endregion

    #region Grid Generation

    /// <summary>
    /// グリッド全体を生成する
    /// </summary>
    public void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX} TilePrefabが設定されていません");
            return;
        }

        // 既存のグリッドを削除
        ClearGrid();

        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateTile(x, y);
            }
        }

        Debug.Log($"{LOG_PREFIX} グリッド生成完了: {width}x{height}");
    }

    /// <summary>
    /// 個別のタイルを生成
    /// </summary>
    private void CreateTile(int x, int y)
    {
        Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
        tileObj.name = $"Tile_{x}_{y}";
        tileObj.transform.localScale = new Vector3(tileScale, 1f, tileScale);

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.LogWarning($"{LOG_PREFIX} TilePrefabにTileコンポーネントがないため追加: ({x}, {y})");
            tile = tileObj.AddComponent<Tile>();
        }

        tile.gridPosition = new Vector2Int(x, y);
        grid[x, y] = tile;
    }

    /// <summary>
    /// グリッドをクリア
    /// </summary>
    private void ClearGrid()
    {
        if (grid != null)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            grid = null;
        }
    }

    #endregion

    #region Grid Access

    /// <summary>
    /// すべてのタイルを取得
    /// </summary>
    public Tile[,] GetAllTiles()
    {
        return grid;
    }

    /// <summary>
    /// 指定した座標のタイルを返す（範囲外ならnull）
    /// </summary>
    public Tile GetTileAt(Vector2Int pos)
    {
        if (!IsInBounds(pos))
            return null;

        return grid[pos.x, pos.y];
    }

    /// <summary>
    /// 指定した座標のタイルを返す（オーバーロード）
    /// </summary>
    public Tile GetTileAt(int x, int y)
    {
        return GetTileAt(new Vector2Int(x, y));
    }

    #endregion

    #region Coordinate Conversion

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
    /// グリッド座標をワールド座標に変換（オーバーロード）
    /// </summary>
    public Vector3 GridToWorld(int x, int y)
    {
        return GridToWorld(new Vector2Int(x, y));
    }

    #endregion

    #region Walkability Check

    /// <summary>
    /// 指定位置が歩行可能かチェック
    /// </summary>
    public bool IsWalkable(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile == null) return false;
        return tile.Walkable;
    }

    /// <summary>
    /// 指定位置が歩行可能かチェック（オーバーロード）
    /// </summary>
    public bool IsWalkable(int x, int y)
    {
        return IsWalkable(new Vector2Int(x, y));
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
        if (grid == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null)
                {
                    tile.ResetColor();
                }
            }
        }

        Debug.Log($"{LOG_PREFIX} すべてのタイルの色をリセット");
    }

    #endregion

    #region Occupant Management

    /// <summary>
    /// 指定位置のOccupantを取得
    /// </summary>
    public GameObject GetOccupantAt(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        return tile?.occupant;
    }

    /// <summary>
    /// 指定位置にOccupantを設定
    /// </summary>
    public void SetOccupantAt(Vector2Int gridPosition, GameObject occupant)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile != null)
        {
            tile.SetOccupantObject(occupant);
        }
    }

    /// <summary>
    /// 指定位置のOccupantをクリア
    /// </summary>
    public void ClearOccupantAt(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile != null)
        {
            tile.ClearOccupant();
        }
    }

    #endregion

    #region Trap Management

    /// <summary>
    /// 指定位置にトラップを設置
    /// </summary>
    public bool SetTrapAt(Vector2Int gridPosition, Trap trap)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile == null) return false;

        if (tile.HasTrap)
        {
            Debug.LogWarning($"{LOG_PREFIX} 既にトラップが設置されています: {gridPosition}");
            return false;
        }

        tile.SetTrap(trap);
        return true;
    }

    /// <summary>
    /// 指定位置のトラップをクリア
    /// </summary>
    public void ClearTrapAt(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        if (tile != null)
        {
            tile.ClearTrap();
        }
    }

    /// <summary>
    /// 指定位置にトラップがあるかチェック
    /// </summary>
    public bool HasTrapAt(Vector2Int gridPosition)
    {
        Tile tile = GetTileAt(gridPosition);
        return tile != null && tile.HasTrap;
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

    /// <summary>
    /// グリッド範囲内かチェック（オーバーロード）
    /// </summary>
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    /// <summary>
    /// グリッドの状態をリセット（色、Occupant、トラップをクリア）
    /// </summary>
    public void ResetAllTiles()
    {
        if (grid == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null)
                {
                    tile.Reset();
                }
            }
        }

        Debug.Log($"{LOG_PREFIX} すべてのタイルをリセット");
    }

    /// <summary>
    /// デバッグ用：グリッドの状態を出力
    /// </summary>
    public void DebugPrintGrid()
    {
        if (grid == null)
        {
            Debug.Log($"{LOG_PREFIX} グリッドが初期化されていません");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"{LOG_PREFIX} グリッド状態:");

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tile = grid[x, y];
                if (tile == null)
                {
                    sb.Append("X ");
                }
                else if (tile.occupant != null)
                {
                    sb.Append("O ");
                }
                else if (tile.HasTrap)
                {
                    sb.Append("T ");
                }
                else
                {
                    sb.Append("· ");
                }
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

    #endregion
}