using UnityEngine;
using System.Collections.Generic;
using takada;

public class NormalAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[NormalAttack]";
    private const int DAMAGE = 1;
    private const int ATTACK_RANGE = 2; // 前方2マス

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;
    [SerializeField] private PlayerStatus playerStatus;

    private bool isAttackMode = false;
    private Vector2Int currentDir = Vector2Int.up;

    private Dictionary<Vector2Int, Tile> allAttackTiles = new();
    private Dictionary<Vector2Int, GameObject> currentEnemies = new();

    private static readonly Vector2Int[] ALL_DIRS =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    // =========================
    // 攻撃開始
    // =========================
    public void Execute()
    {
        if (isAttackMode) return;

        Debug.Log($"{LOG_PREFIX} 攻撃開始");
        isAttackMode = true;

        // 初期方向：プレイヤーの向き
        currentDir = GetForwardDirFromPlayer();
        RotatePlayer(currentDir);

        RefreshAllDirections();
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

        // 敵がいれば攻撃
        if (currentEnemies.ContainsKey(clickPos))
        {
            RotatePlayer(currentDir);
            AttackEnemy(clickPos);
            FinishAttack();
            return;
        }

        // 方向変更
        Vector2Int newDir = GetDirectionFromClick(clickPos);
        if (newDir != Vector2Int.zero)
        {
            currentDir = newDir;
            RotatePlayer(currentDir);
            RefreshAllDirections();
        }
    }

    // =========================
    // 向き制御
    // =========================
    private void RotatePlayer(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;

        Vector3 lookDir = new Vector3(dir.x, 0f, dir.y);
        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        playerMove.transform.rotation = targetRot;
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
    // 方向判定
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
    // 攻撃範囲描画
    // =========================
    private void RefreshAllDirections()
    {
        ClearTiles();
        currentEnemies.Clear();

        Vector2Int playerPos = playerMove.GetGridPosition();

        foreach (Vector2Int dir in ALL_DIRS)
        {
            DrawDirection(playerPos, dir, dir == currentDir);
        }
    }

    private void DrawDirection(Vector2Int playerPos, Vector2Int dir, bool isCurrentDir)
    {
        for (int i = 1; i <= ATTACK_RANGE; i++)
        {
            Vector2Int pos = playerPos + dir * i;
            Tile tile = gridManager.GetTileAt(pos);
            if (tile == null) break;

            allAttackTiles[pos] = tile;

            GameObject enemy = GetEnemyAtPosition(pos);
            if (enemy != null && isCurrentDir)
                currentEnemies[pos] = enemy;

            if (isCurrentDir)
                tile.SetEnemyAttackColor(); // 赤
            else
                tile.SetTargetColor();      // 黄
        }
    }

    // =========================
    // 攻撃処理
    // =========================
    private void AttackEnemy(Vector2Int pos)
    {
        if (!currentEnemies.TryGetValue(pos, out GameObject enemy)) return;

        BattleEnemy be = enemy.GetComponent<BattleEnemy>();
        if (be == null) return;

        be.TakeDamage(DAMAGE);
        Debug.Log($"{LOG_PREFIX} 敵に{DAMAGE}ダメージ");

        // 攻撃成功でSP回復
        playerStatus?.RestoreSP(1);

        turnController.EndPlayerTurn();
    }

    // =========================
    // 敵検索
    // =========================
    private GameObject GetEnemyAtPosition(Vector2Int pos)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy == null) continue;

            Vector2Int ePos = gridManager.WorldToGrid(enemy.transform.position);
            if (ePos == pos)
                return enemy;
        }
        return null;
    }

    // =========================
    // 終了・後始末
    // =========================
    public void CancelAttack()
    {
        Debug.Log($"{LOG_PREFIX} 攻撃キャンセル");
        FinishAttack();
    }

    private void FinishAttack()
    {
        if (!isAttackMode) return;

        ClearTiles();
        currentEnemies.Clear();
        isAttackMode = false;

        Debug.Log($"{LOG_PREFIX} 攻撃モード終了");
    }

    private void ClearTiles()
    {
        foreach (Tile tile in allAttackTiles.Values)
        {
            tile?.ResetColor();
        }
        allAttackTiles.Clear();
    }

    // =========================
    // デバッグ用
    // =========================
    public bool IsAttackMode() => isAttackMode;
    public Vector2Int GetCurrentDirection() => currentDir;
}
