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

    public bool isActionLocked = false;

    private Vector2Int gridPos;
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
        Debug.Log("移動モード有効化");
    }

    public void CancelMove()
    {
        canMove = false;
        isMoving = false;
        ClearMovableTiles();
    }

    private void ShowMovableTiles()
    {
        ClearMovableTiles();

        foreach (Vector2Int direction in MOVE_DIRECTIONS)
        {
            Vector2Int checkPos = gridPos + direction;
            Tile tile = gridManager.GetTileAt(checkPos);

            if (tile != null && tile.walkable)
            {
                tile.SetMovableColor(true);
            }
        }
    }

    private void ClearMovableTiles()
    {
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
        Debug.Log("移動モード無効化");
    }

    private void TryMoveToMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile clickedTile = hit.collider.GetComponent<Tile>();
        if (clickedTile == null) return;

        Vector2Int targetGridPos = clickedTile.gridPosition;

        if (IsAdjacentTile(targetGridPos))
        {
            MoveToTile(clickedTile, targetGridPos);
        }
    }

    private bool IsAdjacentTile(Vector2Int targetGridPos)
    {
        Vector2Int diff = targetGridPos - gridPos;

        return (Mathf.Abs(diff.x) == 1 && diff.y == 0) ||
               (Mathf.Abs(diff.y) == 1 && diff.x == 0);
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