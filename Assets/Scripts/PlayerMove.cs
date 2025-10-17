using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GridManager gridManager;
    public float moveSpeed = 2f; // 移動速度（1秒で進む距離）
    public float yOffset = 0.5f;

    private Vector2Int gridPos;
    private Vector3 targetPos;   // 移動先のワールド座標

    void Start()
    {
        gridPos = new Vector2Int(0, 0);

        Tile startTile = gridManager.GetTileAt(gridPos);
        if (startTile != null)
        {
            targetPos = startTile.transform.position + new Vector3(0, yOffset, 0);
            transform.position = targetPos;
        }
        else
        {
            Debug.LogError($"StartTile が null です。gridPos={gridPos} が範囲外かもしれません。");
        }
    }

    void Update()
    {
        // スムーズに移動
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
            MoveToMouseClick();
    }

    void MoveToMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            if (clickedTile != null)
            {
                Vector2Int targetGridPos = clickedTile.gridPosition;
                Vector2Int diff = targetGridPos - gridPos;

                // 斜め無効・1マス以内チェック
                if ((Mathf.Abs(diff.x) == 1 && diff.y == 0) || (Mathf.Abs(diff.y) == 1 && diff.x == 0))
                {
                    gridPos += diff; // gridPos 更新
                    targetPos = clickedTile.transform.position + new Vector3(0, yOffset, 0);
                }
            }
        }
    }
}
