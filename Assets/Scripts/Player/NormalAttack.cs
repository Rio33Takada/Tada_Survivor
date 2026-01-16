using UnityEngine;
using System.Collections.Generic;
using takada;

public class NormalAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[NormalAttack]";

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;

    private bool isAttackMode = false;

    private Dictionary<Vector2Int, Tile> attackTiles = new();
    private HashSet<Vector2Int> enemyTiles = new();

    private Vector2Int currentDir = Vector2Int.up;
    private static readonly Vector2Int[] ALL_DIRS =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public void Execute()
    {
        Debug.Log($"{LOG_PREFIX} 攻撃開始");
        isAttackMode = true;
        currentDir = Vector2Int.up;

        RefreshAllDirections();
    }

    private void Update()
    {
        if (!isAttackMode) return;

        if (Input.GetMouseButtonDown(0))
            TrySelectDirectionOrAttack();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CancelAttack();
    }

    private void RefreshAllDirections()
    {
        ClearTiles();
        enemyTiles.Clear();

        Vector2Int playerPos = playerMove.GetGridPosition();

        foreach (Vector2Int dir in ALL_DIRS)
        {
            bool hasEnemyInDir = false;

            // 縦2マス
            for (int f = 1; f <= 2; f++)
            {
                Vector2Int pos = playerPos + dir * f;
                Tile tile = gridManager.GetTileAt(pos);
                if (tile == null) break;

                attackTiles[pos] = tile;

                if (IsEnemyOnTile(pos))
                {
                    hasEnemyInDir = true;
                    enemyTiles.Add(pos);
                }
            }

            // 色付け
            for (int f = 1; f <= 2; f++)
            {
                Vector2Int pos = playerPos + dir * f;
                if (!attackTiles.ContainsKey(pos)) continue;

                Tile tile = attackTiles[pos];
                if (hasEnemyInDir)
                    tile.SetEnemyAttackColor(); // 赤: 敵方向全体
                else
                    tile.SetTargetColor();      // 黄: 敵なし
            }
        }
    }

    private void TrySelectDirectionOrAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int clickPos = tile.gridPosition;
        Vector2Int playerPos = playerMove.GetGridPosition();

        Vector2Int dir = clickPos - playerPos;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            currentDir = dir.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            currentDir = dir.y > 0 ? Vector2Int.up : Vector2Int.down;

        RefreshAllDirections();

        // 敵がいる場合は攻撃
        if (enemyTiles.Contains(clickPos))
        {
            AttackEnemy(clickPos);
            FinishAttack();
        }
    }

    private void AttackEnemy(Vector2Int pos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (gridManager.WorldToGrid(enemy.transform.position) == pos)
            {
                BattleEnemy be = enemy.GetComponent<BattleEnemy>();
                if (be != null)
                    be.TakeDamage(1);

                Debug.Log($"{LOG_PREFIX} Enemy Hit at {pos}");
                return;
            }
        }
    }

    private bool IsEnemyOnTile(Vector2Int pos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (gridManager.WorldToGrid(enemy.transform.position) == pos)
                return true;
        }
        return false;
    }

    public void CancelAttack()
    {
        FinishAttack();
    }

    private void FinishAttack()
    {
        ClearTiles();
        isAttackMode = false;
    }

    private void ClearTiles()
    {
        foreach (Tile tile in attackTiles.Values)
            tile.ResetColor();

        attackTiles.Clear();
        enemyTiles.Clear();
    }
}
