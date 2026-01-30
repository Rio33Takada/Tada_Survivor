using UnityEngine;
using System.Collections.Generic;
using takada;

public class SpecialAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[SpecialAttack]";
    private const int DAMAGE = 2;
    private const int SP_COST = 1;

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;
    [SerializeField] private PlayerStatus playerStatus;

    [Header("Range")]
    [SerializeField] private int rangeForward = 2; // 前1~2マス（足元除く）
    [SerializeField] private int rangeSide = 1;    // 横1 → 3列

    [Header("Input")]
    [SerializeField] private float doubleClickTime = 0.3f;

    private bool isAttackMode = false;
    private float lastClickTime;

    private Vector2Int currentDir;

    private readonly Dictionary<Vector2Int, Tile> attackTiles = new();
    private readonly HashSet<GameObject> hitEnemies = new();

    private static readonly Vector2Int[] ALL_DIRS =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    // =========================
    // 外部参照（CommandController用）
    // =========================
    public bool IsAttackMode() => isAttackMode;

    // =========================
    // 攻撃開始
    // =========================
    public void Execute()
    {
        if (isAttackMode) return;

        if (!playerStatus.ConsumeSP(SP_COST))
        {
            Debug.Log($"{LOG_PREFIX} SP不足");
            return;
        }

        isAttackMode = true;

        // ★ 前固定しない：現在の向きを取得
        currentDir = GetForwardDirFromPlayer();
        RotatePlayer(currentDir);

        RefreshAllDirections();

        Debug.Log($"{LOG_PREFIX} 開始 dir={currentDir}");
    }

    private void Update()
    {
        if (!isAttackMode) return;

        if (Input.GetMouseButtonDown(0))
            HandleClick();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CancelAttack();
    }

    // =========================
    // 入力処理
    // =========================
    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int clickPos = tile.gridPosition;

        // ダブルクリック判定
        Vector2Int newDir = GetDirectionFromClick(clickPos);
        if (newDir == Vector2Int.zero) return;

        bool isDouble =
            newDir == currentDir &&
            Time.time - lastClickTime < doubleClickTime;

        currentDir = newDir;
        RotatePlayer(currentDir);
        RefreshAllDirections();

        if (isDouble)
            DoAttack();

        lastClickTime = Time.time;
    }

    // =========================
    // 向き制御（NormalAttackと同等）
    // =========================
    private void RotatePlayer(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;

        Vector3 lookDir = new Vector3(dir.x, 0f, dir.y);
        Quaternion rot = Quaternion.LookRotation(lookDir);
        playerMove.transform.rotation = rot;
    }

    private Vector2Int GetForwardDirFromPlayer()
    {
        Vector3 fwd = playerMove.transform.forward;
        fwd.y = 0f;

        if (Mathf.Abs(fwd.x) > Mathf.Abs(fwd.z))
            return fwd.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            return fwd.z > 0 ? Vector2Int.up : Vector2Int.down;
    }

    // =========================
    // 方向判定（クリック）
    // =========================
    private Vector2Int GetDirectionFromClick(Vector2Int clickPos)
    {
        Vector2Int playerPos = playerMove.GetGridPosition();
        Vector2Int diff = clickPos - playerPos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return diff.x > 0 ? Vector2Int.right : Vector2Int.left;
        else if (Mathf.Abs(diff.y) > 0)
            return diff.y > 0 ? Vector2Int.up : Vector2Int.down;

        return Vector2Int.zero;
    }

    // =========================
    // 範囲描画（前方2マス×横3列、足元除く）
    // =========================
    private void RefreshAllDirections()
    {
        ClearTiles();
        hitEnemies.Clear();

        Vector2Int origin = playerMove.GetGridPosition();

        // まず他の方向を黄色で描画
        foreach (var dir in ALL_DIRS)
        {
            if (dir != currentDir)
            {
                DrawDirection(origin, dir, false);
            }
        }

        // 最後に現在の方向を赤色で描画（最優先）
        DrawDirection(origin, currentDir, true);
    }

    private void DrawDirection(Vector2Int origin, Vector2Int dir, bool isCurrent)
    {
        // ★ Grid基準で「横」を計算する
        Vector2Int side;

        if (dir == Vector2Int.up || dir == Vector2Int.down)
        {
            side = Vector2Int.right; // 横はX方向
        }
        else
        {
            side = Vector2Int.up;    // 横はY方向
        }

        // ★ 前方のみ（足元は除外）
        for (int f = 1; f <= rangeForward; f++)
        {
            for (int s = -rangeSide; s <= rangeSide; s++)
            {
                Vector2Int pos = origin + dir * f + side * s;
                Tile tile = gridManager.GetTileAt(pos);
                if (tile == null) continue;

                if (isCurrent)
                {
                    // 現在の方向：赤色で敵を登録（常に上書き）
                    attackTiles[pos] = tile;
                    tile.SetEnemyAttackColor();
                    RegisterEnemy(pos);
                }
                else
                {
                    // 他の方向：まだ登録されていない場合のみ黄色で表示
                    if (!attackTiles.ContainsKey(pos))
                    {
                        attackTiles[pos] = tile;
                        tile.SetTargetColor();
                    }
                }
            }
        }
    }


    private void RegisterEnemy(Vector2Int pos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy == null) continue;

            Vector2Int ePos =
                gridManager.WorldToGrid(enemy.transform.position);

            if (ePos == pos)
                hitEnemies.Add(enemy);
        }
    }

    // =========================
    // 攻撃
    // =========================
    private void DoAttack()
    {
        // 敵がいない場合は攻撃しない
        if (hitEnemies.Count == 0)
        {
            Debug.Log($"{LOG_PREFIX} 攻撃範囲に敵がいません");
            return;
        }

        Debug.Log($"{LOG_PREFIX} 攻撃 対象:{hitEnemies.Count}");

        foreach (var enemy in hitEnemies)
        {
            if (enemy == null) continue;

            var be = enemy.GetComponent<BattleEnemy>();
            if (be != null)
                be.TakeDamage(DAMAGE);
        }

        Finish();
        turnController.EndPlayerTurn();
    }

    // =========================
    // 終了・キャンセル
    // =========================
    public void CancelAttack()
    {
        if (!isAttackMode) return;

        Debug.Log($"{LOG_PREFIX} キャンセル");
        playerStatus.RestoreSP(SP_COST);
        Finish();
    }

    private void Finish()
    {
        ClearTiles();
        hitEnemies.Clear();
        isAttackMode = false;
    }

    private void ClearTiles()
    {
        foreach (var t in attackTiles.Values)
            if (t != null) t.ResetColor();

        attackTiles.Clear();
    }
}