using UnityEngine;
using System.Collections.Generic;
using takada;

public class SpecialAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[SpecialAttack]";
    private const int SP_COST = 1;
    private const int DAMAGE = 2;

    private const int RANGE_FORWARD = 2;
    private const int RANGE_SIDE = 1;

    [Header("References")]
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;

    [Header("Settings")]
    [SerializeField] private float doubleClickTime = 0.3f;

    private bool isAttackMode = false;
    private Vector2Int currentDir = Vector2Int.up;
    private float lastClickTime;

    private Dictionary<Vector2Int, Tile> allAttackTiles = new();
    private HashSet<GameObject> currentHitEnemies = new();

    private static readonly Vector2Int[] ALL_DIRS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    /// <summary>
    /// 特殊攻撃を開始
    /// </summary>
    public void Execute()
    {
        // 既に攻撃モード中なら無視
        if (isAttackMode)
        {
            Debug.Log($"{LOG_PREFIX} 既に攻撃モード中");
            return;
        }

        if (!playerStatus.ConsumeSP(SP_COST))
        {
            Debug.Log($"{LOG_PREFIX} SP不足");
            return;
        }

        isAttackMode = true;
        currentDir = Vector2Int.up;
        lastClickTime = 0f;

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

        Vector2Int clickedDir = GetDirectionFromTile(tile);
        if (clickedDir == Vector2Int.zero) return;

        // ダブルクリック判定（同じ方向を再度クリック）
        bool isDoubleClick = clickedDir == currentDir &&
                            (Time.time - lastClickTime < doubleClickTime);

        currentDir = clickedDir;
        RefreshAllDirections();

        if (isDoubleClick)
        {
            DoAttack();
        }

        lastClickTime = Time.time;
    }

    /// <summary>
    /// クリックされたタイルから方向を取得
    /// </summary>
    private Vector2Int GetDirectionFromTile(Tile tile)
    {
        Vector2Int playerPos = playerMove.GetGridPosition();
        Vector2Int diff = tile.gridPosition - playerPos;

        // 4方向のいずれかに正規化
        Vector2Int dir = new Vector2Int(
            Mathf.Clamp(diff.x, -1, 1),
            Mathf.Clamp(diff.y, -1, 1)
        );

        // 4方向（上下左右）のみ有効
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) != 1)
            return Vector2Int.zero;

        return dir;
    }

    /// <summary>
    /// 全方向の攻撃範囲を表示（選択方向=赤、その他=黄色）
    /// </summary>
    private void RefreshAllDirections()
    {
        ClearTiles();
        currentHitEnemies.Clear();

        Vector2Int playerPos = playerMove.GetGridPosition();

        // 先に非選択方向を描画（黄色）
        foreach (Vector2Int dir in ALL_DIRS)
        {
            if (dir != currentDir)
            {
                DrawDirection(playerPos, dir, false);
            }
        }

        // 最後に選択方向を描画（赤）- 上書きして確実に赤にする
        DrawDirection(playerPos, currentDir, true);
    }

    /// <summary>
    /// 指定方向の攻撃範囲を描画
    /// </summary>
    private void DrawDirection(Vector2Int playerPos, Vector2Int dir, bool isMainDir)
    {
        Vector2Int right = new Vector2Int(dir.y, -dir.x);

        for (int f = 1; f <= RANGE_FORWARD; f++)
        {
            for (int s = -RANGE_SIDE; s <= RANGE_SIDE; s++)
            {
                Vector2Int pos = playerPos + dir * f + right * s;
                Tile tile = gridManager.GetTileAt(pos);
                if (tile == null) continue;

                // メイン方向の場合は上書き、それ以外は初回のみ追加
                if (isMainDir || !allAttackTiles.ContainsKey(pos))
                {
                    allAttackTiles[pos] = tile;

                    // 色設定
                    if (isMainDir)
                        tile.SetEnemyAttackColor();
                    else
                        tile.SetTargetColor();
                }

                // 選択方向の敵のみヒットリストに追加
                if (isMainDir)
                {
                    CheckEnemyAtPosition(pos);
                }
            }
        }
    }

    /// <summary>
    /// 指定座標に敵がいるかチェック
    /// </summary>
    private void CheckEnemyAtPosition(Vector2Int pos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector2Int enemyPos = gridManager.WorldToGrid(enemy.transform.position);
            if (enemyPos == pos)
            {
                currentHitEnemies.Add(enemy);
            }
        }
    }

    /// <summary>
    /// 攻撃実行
    /// </summary>
    private void DoAttack()
    {
        Debug.Log($"{LOG_PREFIX} 特殊攻撃確定 - 方向:{currentDir}, 対象数:{currentHitEnemies.Count}");

        int hitCount = 0;
        foreach (GameObject enemy in currentHitEnemies)
        {
            if (enemy == null) continue;

            BattleEnemy be = enemy.GetComponent<BattleEnemy>();
            if (be != null)
            {
                be.TakeDamage(DAMAGE);
                hitCount++;
            }
        }

        Debug.Log($"{LOG_PREFIX} {hitCount}体にダメージを与えました");
        Finish();

        turnController.EndPlayerTurn();
    }

    /// <summary>
    /// 攻撃キャンセル（外部からも呼べる）
    /// </summary>
    public void CancelAttack()
    {
        if (!isAttackMode) return;

        Debug.Log($"{LOG_PREFIX} 攻撃キャンセル");

        // SPを返却
        if (playerStatus != null)
        {
            playerStatus.RestoreSP(SP_COST);
        }

        Finish();
    }

    /// <summary>
    /// 攻撃モード終了
    /// </summary>
    private void Finish()
    {
        if (!isAttackMode) return; // 既に終了していれば何もしない

        ClearTiles();
        currentHitEnemies.Clear();
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
    public int GetTargetCount() => currentHitEnemies.Count;
}