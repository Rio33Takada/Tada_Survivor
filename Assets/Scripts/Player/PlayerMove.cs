using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const string LOG_PREFIX = "[PlayerMove]";

    [Header("References")]
    public GridManager gridManager;
    public TurnController turnController;

    [Header("Move Settings")]
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private float rotateDuration = 0.15f;
    [SerializeField] private int moveRange = 3;
    [SerializeField] private float doubleClickThreshold = 0.3f;

    public bool isActionLocked = false;

    public Vector2Int gridPos { get; private set; }

    private bool isMoving = false;
    private bool canMove = false;
    private float lastClickTime;

    private HashSet<Vector2Int> reachableTiles = new();
    private Queue<Vector2Int> movePath = new();

    private static readonly Vector2Int[] MOVE_DIRS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    #region Unity

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        if (isMoving) return;
        if (!CanAcceptInput()) return;

        HandleInput();
    }

    #endregion

    #region Initialize

    private void InitializePlayer()
    {
        gridPos = Vector2Int.zero;

        Tile start = gridManager.GetTileAt(gridPos);
        transform.position = GetTilePosition(start);

        Debug.Log($"{LOG_PREFIX} 初期化完了 {gridPos}");
    }

    #endregion

    #region Input

    private bool CanAcceptInput()
    {
        return !isActionLocked &&
               turnController != null &&
               turnController.IsPlayerTurn;
    }

    private void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (canMove)
        {
            TryMoveByClick();
        }
        else if (IsPlayerClicked())
        {
            float interval = Time.time - lastClickTime;
            if (interval < doubleClickThreshold)
                EnableMoveMode();

            lastClickTime = Time.time;
        }
    }

    private bool IsPlayerClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit)
               && hit.collider.gameObject == gameObject;
    }

    #endregion

    #region Move Mode

    private void EnableMoveMode()
    {
        canMove = true;
        ShowMovableTiles();
        Debug.Log($"{LOG_PREFIX} 移動モードON");
    }

    public void CancelMove()
    {
        canMove = false;
        movePath.Clear();
        ClearMovableTiles();
    }

    #endregion

    #region Movement

    private void TryMoveByClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (!reachableTiles.Contains(tile.gridPosition)) return;

        BuildStraightPath(tile.gridPosition);
        _ = MovePathAsync();
    }

    private async Task MovePathAsync()
    {
        isMoving = true;
        canMove = false;
        ClearMovableTiles();

        while (movePath.Count > 0)
        {
            Vector2Int next = movePath.Dequeue();
            Tile tile = gridManager.GetTileAt(next);

            if (tile == null || !tile.Walkable)
            {
                CancelMove();
                break;
            }

            Vector3 targetPos = GetTilePosition(tile);
            Vector3 dir = targetPos - transform.position;
            dir.y = 0f;

            await RotateAsync(dir);
            await MoveAsync(targetPos);

            gridPos = next;
        }

        isMoving = false;
        Debug.Log($"{LOG_PREFIX} 移動完了 {gridPos}");
        turnController?.EndPlayerTurn();
    }

    #endregion

    #region Animation

    private Task RotateAsync(Vector3 forward)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(RotateCoroutine(forward, tcs));
        return tcs.Task;
    }

    private IEnumerator RotateCoroutine(Vector3 forward, TaskCompletionSource<bool> tcs)
    {
        if (forward == Vector3.zero)
        {
            tcs.SetResult(true);
            yield break;
        }

        Quaternion start = transform.rotation;
        Quaternion target = Quaternion.LookRotation(forward);

        float time = 0f;
        while (time < rotateDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / rotateDuration);
            transform.rotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        transform.rotation = target;
        tcs.SetResult(true);
    }

    private Task MoveAsync(Vector3 targetPos)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(MoveCoroutine(targetPos, tcs));
        return tcs.Task;
    }

    private IEnumerator MoveCoroutine(Vector3 target, TaskCompletionSource<bool> tcs)
    {
        Vector3 start = transform.position;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / moveDuration);
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        tcs.SetResult(true);
    }

    #endregion

    #region Path / Tile

    private void BuildStraightPath(Vector2Int target)
    {
        movePath.Clear();
        Vector2Int current = gridPos;

        while (current.y != target.y)
        {
            current += current.y < target.y ? Vector2Int.up : Vector2Int.down;
            movePath.Enqueue(current);
        }
        while (current.x != target.x)
        {
            current += current.x < target.x ? Vector2Int.right : Vector2Int.left;
            movePath.Enqueue(current);
        }
    }

    private void ShowMovableTiles()
    {
        reachableTiles.Clear();
        Queue<(Vector2Int pos, int cost)> q = new();
        HashSet<Vector2Int> visited = new();

        q.Enqueue((gridPos, 0));
        visited.Add(gridPos);

        while (q.Count > 0)
        {
            var (pos, cost) = q.Dequeue();
            if (cost >= moveRange) continue;

            foreach (var d in MOVE_DIRS)
            {
                Vector2Int next = pos + d;
                if (visited.Contains(next)) continue;

                Tile t = gridManager.GetTileAt(next);
                if (t == null || !t.Walkable) continue;

                t.SetMovableColor(true);
                reachableTiles.Add(next);
                visited.Add(next);
                q.Enqueue((next, cost + 1));
            }
        }
    }

    private void ClearMovableTiles()
    {
        foreach (Tile t in gridManager.GetAllTiles())
            if (t != null) t.SetMovableColor(false);

        reachableTiles.Clear();
    }

    #endregion

    #region Utility / API

    private Vector3 GetTilePosition(Tile tile)
    {
        return tile.transform.position + Vector3.up * yOffset;
    }

    public Vector2Int GetGridPosition() => gridPos;
    public bool IsMoving() => isMoving;
    public bool IsMoveMode() => canMove;

    #endregion
}
