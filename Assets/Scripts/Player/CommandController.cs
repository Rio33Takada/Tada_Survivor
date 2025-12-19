using UnityEngine;
using System.Collections.Generic;

public class CommandController : MonoBehaviour
{
    private const string LOG_PREFIX = "[CommandController]";
    private const int TRAP_RANGE = 2;

    [Header("References")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;
    [SerializeField] private GridManager gridManager;

    [Header("Trap Settings")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private Color selectableTileColor = new Color(0.5f, 1f, 0.5f, 0.5f);

    private bool isTrapMode = false;
    private List<Vector2Int> selectableTrapPositions = new List<Vector2Int>();
    private HashSet<Vector2Int> occupiedTrapPositions = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, Color> originalTileColors = new Dictionary<Vector2Int, Color>();

    #region Unity Lifecycle

    private void Update()
    {
        if (isTrapMode)
        {
            HandleTrapPlacement();
        }
    }

    #endregion

    #region Command Handlers

    public void OnEndSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} End Selected");

        playerMove.CancelMove();
        turnController.EndPlayerTurn();
    }

    public void OnAttackSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Attack Menu Selected");

        playerMove.CancelMove();
    }

    public void OnNomalAttackSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Normal Attack Selected");

        // 通常攻撃の処理をここに実装
        ExecuteNormalAttack();
    }

    public void OnSpecialAttackSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Special Attack Selected");

        // 特殊攻撃の処理をここに実装
        ExecuteSpecialAttack();
    }

    public void OnSetTrapSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Trap Selected");

        playerMove.CancelMove();

        // トラップ設置の処理をここに実装
        ExecuteTrapSetup();
    }

    public void CancelTrapMode()
    {
        if (!isTrapMode) return;

        Debug.Log($"{LOG_PREFIX} トラップ設置範囲非表示");

        isTrapMode = false;
        ResetTileColors();
        selectableTrapPositions.Clear();
    }

    #endregion

    #region Command Execution

    private void ExecuteNormalAttack()
    {
        // 通常攻撃のロジック
        Debug.Log($"{LOG_PREFIX} Executing Normal Attack");

        // 例：攻撃範囲の表示、ターゲット選択など
    }

    private void ExecuteSpecialAttack()
    {
        // 特殊攻撃のロジック
        Debug.Log($"{LOG_PREFIX} Executing Special Attack");

        // 例：スキルポイントの消費、特殊効果の適用など
    }

    private void ExecuteTrapSetup()
    {
        Debug.Log($"{LOG_PREFIX} トラップ設置範囲表示");

        if (trapPrefab == null)
        {
            Debug.LogError($"{LOG_PREFIX} Trap Prefab is not assigned!");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError($"{LOG_PREFIX} GridManager is not assigned!");
            return;
        }

        // トラップモード開始
        isTrapMode = true;

        // 設置可能な位置を計算
        CalculateSelectableTrapPositions();

        // タイルの色を変更
        HighlightSelectableTiles();
    }

    #endregion

    #region Trap Placement Logic

    private void CalculateSelectableTrapPositions()
    {
        selectableTrapPositions.Clear();

        Vector2Int playerPosition = gridManager.WorldToGrid(playerMove.transform.position);

        // プレイヤーから半径2マス以内の位置を計算
        for (int x = -TRAP_RANGE; x <= TRAP_RANGE; x++)
        {
            for (int y = -TRAP_RANGE; y <= TRAP_RANGE; y++)
            {
                // マンハッタン距離で半径2以内
                if (Mathf.Abs(x) + Mathf.Abs(y) <= TRAP_RANGE)
                {
                    Vector2Int checkPosition = playerPosition + new Vector2Int(x, y);

                    // プレイヤーの位置は除外
                    if (checkPosition == playerPosition) continue;

                    // すでにトラップが設置されている位置は除外
                    if (occupiedTrapPositions.Contains(checkPosition)) continue;

                    // 移動可能なタイルかチェック
                    if (gridManager.IsWalkable(checkPosition))
                    {
                        selectableTrapPositions.Add(checkPosition);
                    }
                }
            }
        }
    }

    private void HighlightSelectableTiles()
    {
        originalTileColors.Clear();

        foreach (Vector2Int position in selectableTrapPositions)
        {
            // 元の色を保存
            Color originalColor = gridManager.GetTileColor(position);
            originalTileColors[position] = originalColor;

            // 選択可能な色に変更
            gridManager.SetTileColor(position, selectableTileColor);
        }
    }

    private void ResetTileColors()
    {
        foreach (var kvp in originalTileColors)
        {
            gridManager.SetTileColor(kvp.Key, kvp.Value);
        }

        originalTileColors.Clear();
    }

    private void HandleTrapPlacement()
    {
        // マウスクリックでトラップを設置
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2Int gridPosition = gridManager.WorldToGrid(hit.point);

                if (selectableTrapPositions.Contains(gridPosition))
                {
                    PlaceTrap(gridPosition);
                }
            }
        }

        // 右クリックまたはEscapeでキャンセル
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelTrapMode();
        }
    }

    private void PlaceTrap(Vector2Int gridPosition)
    {
        Vector3 worldPosition = gridManager.GridToWorld(gridPosition);

        // トラップを生成
        GameObject trap = Instantiate(trapPrefab, worldPosition, Quaternion.identity);
        trap.name = $"Trap_{gridPosition.x}_{gridPosition.y}";

        // 設置済み位置として記録
        occupiedTrapPositions.Add(gridPosition);

        Debug.Log($"{LOG_PREFIX} トラップ設置位置 {gridPosition}");

        // トラップモードを終了
        CancelTrapMode();

        // ★ トラップ設置後ターン終了
        turnController.EndPlayerTurn();
    }


    public void ClearAllTraps()
    {
        occupiedTrapPositions.Clear();
        Debug.Log($"{LOG_PREFIX} All trap positions cleared");
    }

    #endregion

    #region Validation

    private bool ValidatePlayerTurn()
    {
        if (turnController == null)
        {
            Debug.LogError($"{LOG_PREFIX} TurnController is not assigned!");
            return false;
        }

        if (!turnController.IsPlayerTurn)
        {
            Debug.LogWarning($"{LOG_PREFIX} It's not player's turn!");
            return false;
        }

        return true;
    }

    #endregion
}