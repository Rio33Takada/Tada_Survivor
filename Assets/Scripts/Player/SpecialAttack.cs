using UnityEngine;
using System.Collections.Generic;
using takada;

public class SpecialAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[SpecialAttack]";
    private const int SP_COST = 1;
    private const int DAMAGE = 2;

    private const int RANGE_FORWARD = 2; // 縦
    private const int RANGE_SIDE = 1;    // 横（±1 → 3マス）

    [Header("References")]
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;

    private bool isAttackMode = false;
    private Vector2Int currentDir = Vector2Int.up;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = 0.3f;

    private Dictionary<Vector2Int, Tile> attackTiles = new();
    private HashSet<GameObject> hitEnemies = new();

    private static readonly Vector2Int[] ALL_DIRS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public void Execute()
    {
        if (!playerStatus.ConsumeSP(SP_COST))
        {
            Debug.Log($"{LOG_PREFIX} SP不足");
            return;
        }

        isAttackMode = true;
        currentDir = Vector2Int.up;

        RefreshAllDirections();
    }

    private void Update()
    {
        if (!isAttackMode) return;

        if (Input.GetMouseButtonDown(0))
            HandleClick();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int playerPos = playerMove.GetGridPosition();
        Vector2Int dir = tile.gridPosition - playerPos;

        dir = new Vector2Int(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1)
        );

        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) != 1) return;

        currentDir = dir;
        RefreshAllDirections();

        if (Time.time - lastClickTime < DOUBLE_CLICK_TIME)
        {
            DoAttack();
        }

        lastClickTime = Time.time;
    }

    /// <summary>
    /// 全方向を描画（選択方向＝赤、それ以外＝黄色）
    /// </summary>
    private void RefreshAllDirections()
    {
        ClearTiles();
        hitEnemies.Clear();

        foreach (Vector2Int dir in ALL_DIRS)
        {
            bool isMainDir = dir == currentDir;
            DrawDirection(dir, isMainDir);
        }
    }

    private void DrawDirection(Vector2Int dir, bool isMainDir)
    {
        Vector2Int playerPos = playerMove.GetGridPosition();
        Vector2Int right = new Vector2Int(dir.y, -dir.x);

        for (int f = 1; f <= RANGE_FORWARD; f++)
        {
            for (int s = -RANGE_SIDE; s <= RANGE_SIDE; s++)
            {
                Vector2Int pos = playerPos + dir * f + right * s;
                Tile tile = gridManager.GetTileAt(pos);
                if (tile == null) continue;

                attackTiles[pos] = tile;

                if (isMainDir)
                {
                    tile.SetEnemyAttackColor(); // 赤
                }
                else
                {
                    tile.SetTargetColor();      // 黄
                }

                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if (gridManager.WorldToGrid(enemy.transform.position) == pos)
                    {
                        hitEnemies.Add(enemy);
                    }
                }
            }
        }
    }

    private void DoAttack()
    {
        Debug.Log($"{LOG_PREFIX} 特殊攻撃確定");

        foreach (GameObject enemy in hitEnemies)
        {
            BattleEnemy be = enemy.GetComponent<BattleEnemy>();
            if (be != null)
            {
                be.TakeDamage(DAMAGE);
            }
        }

        Finish();
    }

    private void Cancel()
    {
        Finish();
    }

    private void Finish()
    {
        ClearTiles();
        isAttackMode = false;
    }

    private void ClearTiles()
    {
        foreach (Tile tile in attackTiles.Values)
            tile.ResetColor();

        attackTiles.Clear();
    }
}
