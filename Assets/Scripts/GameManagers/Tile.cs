using UnityEngine;

public class Tile : MonoBehaviour
{
    private const string LOG_PREFIX = "[Tile]";

    [Header("Grid Position")]
    public Vector2Int gridPosition;

    [Header("Occupant")]
    public GameObject occupant;

    [Header("Trap")]
    [SerializeField] private Trap trap;

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color moveColor = Color.cyan;
    [SerializeField] private Color attackRangeColor = new Color(1f, 0f, 0f, 0.4f);
    [SerializeField] private Color enemyAttackColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] private Color targetColor = Color.yellow;

    private Renderer rend;
    private Color currentColor;

    /// <summary>
    /// 何もいなければ歩ける
    /// </summary>
    public bool Walkable => occupant == null;

    /// <summary>
    /// トラップが設置されているか
    /// </summary>
    public bool HasTrap => trap != null;

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeRenderer();
    }

    #endregion

    #region Initialization

    private void InitializeRenderer()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            return;
        }

        currentColor = defaultColor;
        rend.material.color = currentColor;
    }

    #endregion

    #region Color Management

    /// <summary>
    /// 移動可能タイルの色を設定
    /// </summary>
    public void SetMovableColor(bool enable)
    {
        SetColor(enable ? moveColor : defaultColor);
    }

    /// <summary>
    /// 攻撃範囲タイルの色を設定
    /// </summary>
    public void SetAttackRangeColor(bool enable)
    {
        SetColor(enable ? attackRangeColor : defaultColor);
    }

    /// <summary>
    /// 敵攻撃タイルの色を設定（赤色）
    /// </summary>
    public void SetEnemyAttackColor()
    {
        SetColor(enemyAttackColor);
    }

    /// <summary>
    /// ターゲットタイルの色を設定（黄色）
    /// </summary>
    public void SetTargetColor()
    {
        SetColor(targetColor);
    }

    /// <summary>
    /// デフォルト色にリセット
    /// </summary>
    public void ResetColor()
    {
        SetColor(defaultColor);
    }

    /// <summary>
    /// 色を設定する内部メソッド
    /// </summary>
    private void SetColor(Color color)
    {
        if (rend == null) return;

        currentColor = color;
        rend.material.color = color;
    }

    #endregion

    #region Trap Management

    /// <summary>
    /// トラップを設置
    /// </summary>
    public void SetTrap(Trap t)
    {
        if (t == null)
        {
            Debug.LogWarning($"{LOG_PREFIX} nullのトラップを設置しようとしました: {gridPosition}");
            return;
        }

        if (trap != null)
        {
            Debug.LogWarning($"{LOG_PREFIX} 既にトラップが設置されています: {gridPosition}");
            return;
        }

        trap = t;
        Debug.Log($"{LOG_PREFIX} トラップ設置: {gridPosition}");
    }

    /// <summary>
    /// トラップを除去
    /// </summary>
    public void ClearTrap()
    {
        if (trap != null)
        {
            Debug.Log($"{LOG_PREFIX} トラップ除去: {gridPosition}");
            trap = null;
        }
    }

    #endregion

    #region Occupant Management

    /// <summary>
    /// タイルに配置されているオブジェクトを設定
    /// </summary>
    public void SetOccupantObject(GameObject obj)
    {
        occupant = obj;

        if (obj != null)
        {
            Debug.Log($"{LOG_PREFIX} occupant設定: {obj.name} at {gridPosition}, trap: {trap != null}");

            // トラップがある場合、敵が踏んだら発動
            if (trap != null)
            {
                TryActivateTrap(obj);
            }
        }
        else
        {
            Debug.Log($"{LOG_PREFIX} occupant解除: {gridPosition}");
        }
    }

    /// <summary>
    /// トラップの発動を試みる
    /// </summary>
    private void TryActivateTrap(GameObject obj)
    {
        var enemy = obj.GetComponent<takada.BattleEnemy>();
        if (enemy != null)
        {
            Debug.Log($"{LOG_PREFIX} トラップ発動！ 対象: {obj.name}");
            trap.OnStepped(enemy);
            trap = null; // トラップは一度使用したら消える
        }
    }

    /// <summary>
    /// Occupantをクリア
    /// </summary>
    public void ClearOccupant()
    {
        if (occupant != null)
        {
            Debug.Log($"{LOG_PREFIX} occupantクリア: {occupant.name} at {gridPosition}");
            occupant = null;
        }
    }

    #endregion

    #region Public Utility

    /// <summary>
    /// タイルの状態をリセット
    /// </summary>
    public void Reset()
    {
        ClearOccupant();
        ClearTrap();
        ResetColor();
    }

    /// <summary>
    /// タイルの詳細情報を取得（デバッグ用）
    /// </summary>
    public string GetDebugInfo()
    {
        return $"Tile[{gridPosition}] Walkable:{Walkable} Occupant:{occupant?.name ?? "None"} Trap:{HasTrap}";
    }

    #endregion
}