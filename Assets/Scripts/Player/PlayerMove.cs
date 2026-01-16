using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
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

    private HashSet<Vector2Int> reachableTiles = new HashSet<Vector2Int>();
    private Queue<Vector2Int> movePath = new Queue<Vector2Int>();


    public Vector2Int gridPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool canMove = false;
    private float lastClickTime = 0f;

    private static readonly Vector2Int[] MOVE_DIRECTIONS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

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

    private void InitializePlayer()
    {
        gridPos = Vector2Int.zero;

        if (gridManager == null)
        {
            Debug.LogError("GridManagerが設定されていません。");
            return;
        }

        Tile startTile = gridManager.GetTileAt(gridPos);
        if (startTile != null)
        {
            targetPos = GetTilePosition(startTile);
            transform.position = targetPos;
        }
    }

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

        if (movePath.Count > 0)
        {
            MoveNextStep();
            return;
        }

        canMove = false;
        ClearMovableTiles();

        Debug.Log($"プレイヤー移動完了: {gridPos}");

        if (turnController != null)
        {
            turnController.EndPlayerTurn();
        }
    }


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

    public void EnableMoveMode()
    {
        canMove = true;
        ShowMovableTiles();
        Debug.Log("移動範囲表示");
    }

    public void CancelMove()
    {
        canMove = false;
        isMoving = false;
        ClearMovableTiles();
    }

    private void BuildStraightPath(Vector2Int target)
    {
        movePath.Clear();

        Vector2Int current = gridPos;

        // ① 縦方向を先に合わせる
        while (current.y != target.y)
        {
            current += current.y < target.y ? Vector2Int.up : Vector2Int.down;
            movePath.Enqueue(current);
        }

        // ② 横方向を合わせる
        while (current.x != target.x)
        {
            current += current.x < target.x ? Vector2Int.right : Vector2Int.left;
            movePath.Enqueue(current);
        }
    }


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
            if (cost >= moveRange) continue;

            foreach (Vector2Int dir in MOVE_DIRECTIONS)
            {
                Vector2Int nextPos = currentPos + dir;
                if (visited.Contains(nextPos)) continue;

                Tile tile = gridManager.GetTileAt(nextPos);
                if (tile == null) continue;

                // ★ ここが重要（敵・物がいるマスを除外）
                if (!tile.Walkable) continue;

                tile.SetMovableColor(true);
                reachableTiles.Add(nextPos);

                visited.Add(nextPos);
                queue.Enqueue((nextPos, cost + 1));
            }
        }
    }

    public void MoveNextStep()
    {
        if (movePath.Count == 0) return;

        if (!isMoving && turnController != null)
        {
            turnController.ClearAllButtons();
        }

        Vector2Int nextPos = movePath.Dequeue();
        Tile tile = gridManager.GetTileAt(nextPos);

        if (tile == null || !tile.Walkable)
        {
            movePath.Clear();
            return;
        }

        gridPos = nextPos;
        targetPos = GetTilePosition(tile);
        isMoving = true;

        Debug.Log($"1マス移動: {gridPos}");
    }


    private void ClearMovableTiles()
    {
        reachableTiles.Clear();

        Tile[,] allTiles = gridManager.GetAllTiles();
        int width = allTiles.GetLength(0);
        int height = allTiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allTiles[x, y].SetMovableColor(false);
            }
        }

        Debug.Log("移動範囲非表示");
    }


    private void TryMoveToMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile clickedTile = hit.collider.GetComponent<Tile>();
        if (clickedTile == null) return;

        Vector2Int targetGridPos = clickedTile.gridPosition;

        if (!IsReachableTile(targetGridPos)) return;

        BuildStraightPath(targetGridPos);
        MoveNextStep();
    }


    private bool IsReachableTile(Vector2Int targetGridPos)
    {
        return reachableTiles.Contains(targetGridPos);
    }




    private void MoveToTile(Tile tile, Vector2Int newGridPos)
    {
        gridPos = newGridPos;
        targetPos = GetTilePosition(tile);
        isMoving = true;

        Debug.Log($"プレイヤー移動開始: {gridPos}");
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

    private Vector3 GetTilePosition(Tile tile)
    {
        return tile.transform.position + new Vector3(0, yOffset, 0);
    }

    public Vector2Int GetGridPosition()
    {
        return gridPos;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}