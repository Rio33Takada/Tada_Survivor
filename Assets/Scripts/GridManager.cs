using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab; // マスのPrefab
    public int width = 10;        // 横のマス数
    public int height = 10;       // 縦のマス数
    public float tileScale = 0.099f; // マスの大きさ
    public float spacing = 1.0f;     // マス間の距離

    private Tile[,] grid;

    private void Awake()
    {
        GenerateGrid();
    }

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
                tile.walkable = true;

                grid[x, y] = tile;
            }
        }

        Debug.Log($"Grid生成完了: {width}x{height}");
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
}
