using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public TurnController turnController;

    [Header("Move Settings")]
    public float moveSpeed = 2f;
    public float yOffset = 0.5f;

    private Vector2Int gridPos;
    private Vector3 targetPos;
    private bool isMoving = false;

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
        // 移動中なら補間
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 到着したら停止
            if (Vector3.Distance(transform.position, targetPos) < 0.001f)
            {
                isMoving = false;
                Debug.Log("プレイヤーの移動終了");

                // ターンを敵に渡す
                turnController?.EndPlayerTurn();
            }
        }

        // プレイヤーターン中のみクリック操作を受け付ける
        if (!isMoving && turnController != null && turnController.IsPlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
                TryMoveToMouseClick();
        }
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
            if ((Mathf.Abs(diff.x) == 1 && diff.y == 0) || (Mathf.Abs(diff.y) == 1 && diff.x == 0))
            {
                gridPos = targetGridPos;
                targetPos = clickedTile.transform.position + new Vector3(0, yOffset, 0);
                isMoving = true;
                Debug.Log($"プレイヤー移動: {gridPos}");
            }
        }
    }
}
