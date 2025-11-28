using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public TurnController turnController;

    [Header("Move Settings")]
    public float moveSpeed = 2f;
    public float yOffset = 0.5f;

    public Vector2Int gridPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool canMove = false;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f; // ダブルクリック判定時間

    void Start()
    {
        gridPos = new Vector2Int(0, 0);

        if (gridManager == null)
        {
            Debug.LogError("GridManagerが設定されていません。");
            return;
        }

        Tile startTile = gridManager.GetTileAt(gridPos);
        if (startTile != null)
        {
            targetPos = startTile.transform.position + new Vector3(0, yOffset, 0);
            transform.position = targetPos;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isMoving = false;
                canMove = false;
                ClearMovableTiles(); // ★ 色を戻す
                Debug.Log("プレイヤーの移動終了");
                turnController?.EndPlayerTurn();
            }

        }


        // プレイヤーターン中のみクリック操作を受け付ける
        if (!isMoving && canMove && turnController != null && turnController.IsPlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
                TryMoveToMouseClick();
        }

        // プレイヤーターン中のみ、プレイヤーをダブルクリックで移動範囲表示
        if (turnController != null && turnController.IsPlayerTurn && Input.GetMouseButtonDown(0))
        {
            if (IsPlayerClicked())
            {
                if (Time.time - lastClickTime < doubleClickThreshold)
                {
                    EnableMoveOnce(); // ★ 移動可能範囲を表示
                }

                lastClickTime = Time.time;
            }
        }


    }

    void ShowMovableTiles()
    {
        ClearMovableTiles();

        Vector2Int[] directions =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = gridPos + dir;
            Tile tile = gridManager.GetTileAt(checkPos);

            if (tile != null && tile.walkable)
            {
                tile.SetMovableColor(true);
            }
        }
    }

    void ClearMovableTiles()
    {
        Tile[,] allTiles = gridManager.GetAllTiles();

        for (int x = 0; x < allTiles.GetLength(0); x++)
        {
            for (int y = 0; y < allTiles.GetLength(1); y++)
            {
                allTiles[x, y].SetMovableColor(false);
            }
        }
    }

    public void CancelMove()
    {
        canMove = false;
        isMoving = false;
        ClearMovableTiles(); // 色を元に戻す
    }

    bool IsPlayerClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == this.gameObject)
                return true;
        }
        return false;
    }

    public void EnableMoveOnce()
    {
        canMove = true;
        ShowMovableTiles();
    }

    void TryMoveToMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            if (clickedTile == null) return;

            Vector2Int targetGridPos = clickedTile.gridPosition;
            Vector2Int diff = targetGridPos - gridPos;

            // 上下左右の1マスのみ移動可
            if ((Mathf.Abs(diff.x) == 1 && diff.y == 0) ||
                (Mathf.Abs(diff.y) == 1 && diff.x == 0))
            {
                gridPos = targetGridPos;
                targetPos = clickedTile.transform.position + new Vector3(0, yOffset, 0);
                isMoving = true;
                Debug.Log($"プレイヤー移動: {gridPos}");
            }
        }
    }

}
