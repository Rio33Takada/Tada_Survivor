using UnityEngine;
using System.Collections.Generic;
using takada;

public class NormalAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[NormalAttack]";
    private const int DAMAGE = 1;
    private const int ATTACK_RANGE = 2; // 攻撃範囲（前方2マス）

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

    /// <summary>
    /// 通常攻撃を開始
    /// </summary>
    public void Execute()
    {
        // 既に攻撃モード中なら無視
        if (isAttackMode)
        {
            Debug.Log($"{LOG_PREFIX} 既に攻撃モード中");
            return;
        }

        Debug.Log($"{LOG_PREFIX} 攻撃開始");
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
            CancelAttack();
    }

    /// <summary>
    /// マウスクリック処理
    /// </summary>
    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int clickPos = tile.gridPosition;

        // クリックした位置に敵がいれば攻撃
        if (currentEnemies.ContainsKey(clickPos))
        {
            AttackEnemy(clickPos);
            FinishAttack();
            return;
        }

        // 敵がいなければ方向選択
        Vector2Int newDir = GetDirectionFromClick(clickPos);
        if (newDir != Vector2Int.zero)
        {
            currentDir = newDir;
            RefreshAllDirections();
        }
    }

    /// <summary>
    /// クリック位置から方向を判定
    /// </summary>
    private Vector2Int GetDirectionFromClick(Vector2Int clickPos)
    {
        Vector2Int playerPos = playerMove.GetGridPosition();
        Vector2Int diff = clickPos - playerPos;

        // 横方向と縦方向で大きい方を採用
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return diff.x > 0 ? Vector2Int.right : Vector2Int.left;
        else if (Mathf.Abs(diff.y) > 0)
            return diff.y > 0 ? Vector2Int.up : Vector2Int.down;

        return Vector2Int.zero;
    }

    /// <summary>
    /// 全方向の攻撃範囲を表示
    /// </summary>
    private void RefreshAllDirections()
    {
        ClearTiles();
        currentEnemies.Clear();

        Vector2Int playerPos = playerMove.GetGridPosition();

        // すべての方向を描画（選択方向は最後）
        foreach (Vector2Int dir in ALL_DIRS)
        {
            DrawDirection(playerPos, dir, dir == currentDir);
        }
    }

    /// <summary>
    /// 指定方向の攻撃範囲を描画
    /// </summary>
    private void DrawDirection(Vector2Int playerPos, Vector2Int dir, bool isCurrentDir)
    {
        bool hasEnemy = false;
        List<Vector2Int> directionTiles = new List<Vector2Int>();

        // まず範囲内のタイルと敵をチェック
        for (int f = 1; f <= ATTACK_RANGE; f++)
        {
            Vector2Int pos = playerPos + dir * f;
            Tile tile = gridManager.GetTileAt(pos);
            if (tile == null) break;

            directionTiles.Add(pos);

            GameObject enemy = GetEnemyAtPosition(pos);
            if (enemy != null)
            {
                hasEnemy = true;
                if (isCurrentDir)
                {
                    currentEnemies[pos] = enemy;
                }
            }
        }

        // タイルに色を設定
        foreach (Vector2Int pos in directionTiles)
        {
            Tile tile = gridManager.GetTileAt(pos);
            if (tile == null) continue;

            allAttackTiles[pos] = tile;

            if (isCurrentDir)
            {
                // 選択方向: 常に赤
                tile.SetEnemyAttackColor();
            }
            else
            {
                // 非選択方向: 常に黄色
                tile.SetTargetColor();
            }
        }
    }

    /// <summary>
    /// 指定座標の敵を取得
    /// </summary>
    private GameObject GetEnemyAtPosition(Vector2Int pos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector2Int enemyPos = gridManager.WorldToGrid(enemy.transform.position);
            if (enemyPos == pos)
                return enemy;
        }
        return null;
    }

    /// <summary>
    /// 敵を攻撃
    /// </summary>
    private void AttackEnemy(Vector2Int pos)
    {
        if (!currentEnemies.ContainsKey(pos)) return;

        GameObject enemy = currentEnemies[pos];
        if (enemy == null) return;

        BattleEnemy be = enemy.GetComponent<BattleEnemy>();
        if (be == null) return;

        be.TakeDamage(DAMAGE);
        Debug.Log($"{LOG_PREFIX} 敵に{DAMAGE}ダメージ");

        // ★ 攻撃成功でSP＋1
        if (playerStatus != null)
        {
            playerStatus.RestoreSP(1);
        }

        turnController.EndPlayerTurn();
    }



    /// <summary>
    /// 攻撃キャンセル
    /// </summary>
    public void CancelAttack()
    {
        Debug.Log($"{LOG_PREFIX} 攻撃キャンセル");
        FinishAttack();
    }

    /// <summary>
    /// 攻撃モード終了
    /// </summary>
    private void FinishAttack()
    {
        if (!isAttackMode) return; // 既に終了していれば何もしない

        ClearTiles();
        currentEnemies.Clear();
        isAttackMode = false;

        Debug.Log($"{LOG_PREFIX} 攻撃モード終了");
    }

    /// <summary>
    /// タイルの色をリセット
    /// </summary>
    private void ClearTiles()
    {
        foreach (Tile tile in allAttackTiles.Values)
        {
            if (tile != null)
                tile.ResetColor();
        }

        allAttackTiles.Clear();
    }

    /// <summary>
    /// デバッグ用：現在の状態を取得
    /// </summary>
    public bool IsAttackMode() => isAttackMode;
    public Vector2Int GetCurrentDirection() => currentDir;
    public int GetTargetCount() => currentEnemies.Count;
}