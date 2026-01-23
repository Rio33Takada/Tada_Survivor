using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const string LOG_PREFIX = "[PlayerMove]";

    [Header("References")]
    public GridManager gridManager;
    public TurnController turnController;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float doubleClickThreshold = 0.3f;
    [SerializeField] private float positionThreshold = 0.01f;
    [SerializeField] private int moveRange = 1;

    public bool isActionLocked = false;

    public Vector2Int gridPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool canMove = false;
    private float lastClickTime = 0f;

    private HashSet<Vector2Int> reachableTiles = new HashSet<Vector2Int>();
    private Queue<Vector2Int> movePath = new Queue<Vector2Int>();

    private static readonly Vector2Int[] MOVE_DIRECTIONS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    #region Unity Lifecycle

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        if (isMoving)
        {
            UpdateMovement();
        }
        else if (CanAcceptInput())
        {
            HandleInput();
        }
    }

    #endregion

    #region Initialization

    private void InitializePlayer()
    {
        gridPos = Vector2Int.zero;

        if (gridManager == null)
        {
            Debug.LogError($"{LOG_PREFIX} GridManagerが設定されていません");
            return;
        }

        Tile startTile = gridManager.GetTileAt(gridPos);
        if (startTile != null)
        {
            targetPos = GetTilePosition(startTile);
            transform.position = targetPos;
            Debug.Log($"{LOG_PREFIX} プレイヤー初期化完了: {gridPos}");
        }
        else
        {
            Debug.LogError($"{LOG_PREFIX} 開始タイルが見つかりません: {gridPos}");
        }
    }

    #endregion

    #region Movement

    private void UpdateMovement()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < positionThreshold)
        {
            OnMoveComplete();
        }
    }

    private void OnMoveComplete()
    {
        isMoving = false;

        // まだ移動パスが残っている場合は次のステップへ
        if (movePath.Count > 0)
        {
            MoveNextStep();
            return;
        }

        // 移動完了
        canMove = false;
        ClearMovableTiles();

        Debug.Log($"{LOG_PREFIX} 移動完了: {gridPos}");

        if (turnController != null)
        {
            turnController.EndPlayerTurn();
        }
    }

    private void MoveNextStep()
    {
        if (movePath.Count == 0)
        {
            Debug.LogWarning($"{LOG_PREFIX} 移動パスが空です");
            return;
        }

        // 最初のステップでボタンをクリア
        if (!isMoving && turnController != null)
        {
            turnController.ClearAllButtons();
        }

        Vector2Int nextPos = movePath.Dequeue();
        Tile tile = gridManager.GetTileAt(nextPos);

        // タイルが存在しない、または移動不可の場合は移動を中止
        if (tile == null || !tile.Walkable)
        {
            Debug.LogWarning($"{LOG_PREFIX} 移動不可なタイル: {nextPos}");
            movePath.Clear();
            CancelMove();
            return;
        }

        gridPos = nextPos;
        targetPos = GetTilePosition(tile);
        isMoving = true;

        Debug.Log($"{LOG_PREFIX} 1マス移動: {gridPos}");
    }

    #endregion

    #region Input Handling

    private bool CanAcceptInput()
    {
        return !isActionLocked
            && turnController != null
            && turnController.IsPlayerTurn;
    }

    private void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (canMove)
        {
            TryMoveToMouseClick();
        }
        else if (IsPlayerClicked())
        {
            HandlePlayerClick();
        }
    }

    private void HandlePlayerClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick < doubleClickThreshold)
        {
            EnableMoveMode();
        }

        lastClickTime = Time.time;
    }

    private void TryMoveToMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile clickedTile = hit.collider.GetComponent<Tile>();
        if (clickedTile == null) return;

        Vector2Int targetGridPos = clickedTile.gridPosition;

        // 到達可能なタイルかチェック
        if (!IsReachableTile(targetGridPos))
        {
            Debug.Log($"{LOG_PREFIX} 到達不可能なタイル: {targetGridPos}");
            return;
        }

        BuildStraightPath(targetGridPos);
        MoveNextStep();
    }

    private bool IsPlayerClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    #endregion

    #region Move Mode Control

    /// <summary>
    /// 移動モードを有効化
    /// </summary>
    public void EnableMoveMode()
    {
        if (canMove)
        {
            Debug.Log($"{LOG_PREFIX} 既に移動モード中");
            return;
        }

        canMove = true;
        ShowMovableTiles();
        Debug.Log($"{LOG_PREFIX} 移動モード開始");
    }

    /// <summary>
    /// 移動をキャンセル
    /// </summary>
    public void CancelMove()
    {
        canMove = false;
        isMoving = false;
        movePath.Clear();
        ClearMovableTiles();

        Debug.Log($"{LOG_PREFIX} 移動キャンセル");
    }

    #endregion

    #region Pathfinding

    /// <summary>
    /// 直線経路を構築（縦→横の順）
    /// </summary>
    private void BuildStraightPath(Vector2Int target)
    {
        movePath.Clear();

        Vector2Int current = gridPos;

        // ① 縦方向を先に移動
        while (current.y != target.y)
        {
            current += current.y < target.y ? Vector2Int.up : Vector2Int.down;
            movePath.Enqueue(current);
        }

        // ② 横方向を移動
        while (current.x != target.x)
        {
            current += current.x < target.x ? Vector2Int.right : Vector2Int.left;
            movePath.Enqueue(current);
        }

        Debug.Log($"{LOG_PREFIX} 経路構築: {movePath.Count}マス");
    }

    /// <summary>
    /// 指定位置が到達可能かチェック
    /// </summary>
    private bool IsReachableTile(Vector2Int targetGridPos)
    {
        return reachableTiles.Contains(targetGridPos);
    }

    #endregion

    #region Tile Display

    /// <summary>
    /// 移動可能なタイルを表示
    /// </summary>
    private void ShowMovableTiles()
    {
        ClearMovableTiles();
        reachableTiles.Clear();

        Queue<(Vector2Int pos, int cost)> queue = new Queue<(Vector2Int, int)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue((gridPos, 0));
        visited.Add(gridPos);

        while (queue.Count > 0)
        {
            var (currentPos, cost) = queue.Dequeue();

            // 移動範囲を超えたら探索終了
            if (cost >= moveRange) continue;

            foreach (Vector2Int dir in MOVE_DIRECTIONS)
            {
                Vector2Int nextPos = currentPos + dir;

                // 既に訪問済みならスキップ
                if (visited.Contains(nextPos)) continue;

                Tile tile = gridManager.GetTileAt(nextPos);
                if (tile == null) continue;

                // 移動不可なタイルはスキップ（敵や障害物がいる）
                if (!tile.Walkable) continue;

                // タイルを移動可能として表示
                tile.SetMovableColor(true);
                reachableTiles.Add(nextPos);

                visited.Add(nextPos);
                queue.Enqueue((nextPos, cost + 1));
            }
        }

        Debug.Log($"{LOG_PREFIX} 移動可能範囲: {reachableTiles.Count}マス");
    }

    /// <summary>
    /// 移動可能タイルの表示をクリア
    /// </summary>
    private void ClearMovableTiles()
    {
        if (gridManager == null) return;

        Tile[,] allTiles = gridManager.GetAllTiles();
        if (allTiles == null) return;

        int width = allTiles.GetLength(0);
        int height = allTiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allTiles[x, y] != null)
                {
                    allTiles[x, y].SetMovableColor(false);
                }
            }
        }

        reachableTiles.Clear();
    }

    #endregion

    #region Utility

    private Vector3 GetTilePosition(Tile tile)
    {
        return tile.transform.position + new Vector3(0, yOffset, 0);
    }

    #endregion

    #region Public API

    public Vector2Int GetGridPosition()
    {
        return gridPos;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsMoveMode()
    {
        return canMove;
    }

    #endregion
}