using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // マスのPrefab
    public int width = 10;        // 横のマス数
    public int height = 10;       // 縦のマス数
    public float tileScale = 0.099f; // ← マスの大きさ（すごく小さい）
    public float spacing = 1.0f;    // ← マス同士の間隔を大きくできる

    private Tile[,] grid;

    private void Awake()
    {
        GenerateGrid();
    }
    void Start()
    {
        
    }

    void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // spacing でマス同士の距離を広げる
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity);
                tileObj.name = $"Tile_{x}_{y}";

                // Tileの大きさを小さくする
                tileObj.transform.localScale = new Vector3(tileScale, 1f, tileScale);

                // Tileコンポーネントを追加 or 取得
                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null) tile = tileObj.AddComponent<Tile>();

                tile.gridPosition = new Vector2Int(x, y);
                tile.walkable = true;

                grid[x, y] = tile;
            }
        }
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            return null;

        return grid[pos.x, pos.y];
    }
}
