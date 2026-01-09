using UnityEngine;
using System.Collections.Generic;

public class NormalAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[NormalAttack]";

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;

    private bool isAttackMode = false;

    private Dictionary<Vector2Int, Tile> attackTiles = new();
    private HashSet<Vector2Int> enemyTiles = new();

    public void Execute()
    {
        Debug.Log($"{LOG_PREFIX} ノーマル攻撃開始");

        ClearAttackTiles();
        ShowAttackTiles();
        isAttackMode = true;
    }

    private void Update()
    {
        if (!isAttackMode) return;

        if (Input.GetMouseButtonDown(0))
            TryAttack();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CancelAttack();
    }

    private void ShowAttackTiles()
    {
        Vector2Int playerPos = playerMove.GetGridPosition();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int pos = playerPos + dir;
            Tile tile = gridManager.GetTileAt(pos);

            if (tile == null || !tile.Walkable) continue;

            attackTiles[pos] = tile;

            if (IsEnemyOnTile(pos))
            {
                tile.SetTargetColor();   // 黄色（攻撃可能）
                enemyTiles.Add(pos);
            }
            else
            {
                tile.SetEnemyAttackColor();  // 赤（攻撃不可）
            }
        }
    }

    private void TryAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int clickedPos = tile.gridPosition;

        // Enemy がいるマスのみ攻撃可能
        if (!enemyTiles.Contains(clickedPos))
        {
            Debug.Log($"{LOG_PREFIX} Enemyなし → 攻撃不可");
            return;
        }

        Debug.Log($"{LOG_PREFIX} 攻撃確定 {clickedPos}");

        // ★ ここでダメージ処理
        AttackEnemy(clickedPos);

        FinishAttack();
    }

    private void AttackEnemy(Vector2Int gridPos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (gridManager.WorldToGrid(enemy.transform.position) == gridPos)
            {
                Debug.Log($"{LOG_PREFIX} Enemy Hit!");
                Destroy(enemy); // 仮
                break;
            }
        }
    }

    private bool IsEnemyOnTile(Vector2Int gridPos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (gridManager.WorldToGrid(enemy.transform.position) == gridPos)
                return true;
        }
        return false;
    }

    public void CancelAttack()
    {
        Debug.Log($"{LOG_PREFIX} 攻撃キャンセル");
        FinishAttack();
    }

    private void FinishAttack()
    {
        ClearAttackTiles();
        isAttackMode = false;
    }

    public void ClearAttackTiles()
    {
        foreach (var tile in attackTiles.Values)
            tile.ResetColor();

        attackTiles.Clear();
        enemyTiles.Clear();
    }
}
