using UnityEngine;

public class CommandController : MonoBehaviour
{
    private const string LOG_PREFIX = "[CommandController]";

    [Header("References")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;

    [Header("Commands")]
    [SerializeField] private NormalAttack normalAttack;
    [SerializeField] private SpecialAttack specialAttack;
    [SerializeField] private SetTrap setTrap;

    private void Update()
    {
        // トラップモード中の入力処理は SetTrap に委譲
        if (setTrap != null && setTrap.IsTrapMode)
        {
            setTrap.Tick();
        }
    }

    #region Command Handlers

    public void OnEndSelected()
    {
        if (!ValidateCommand()) return;

        Debug.Log($"{LOG_PREFIX} End Selected");
        CancelAllModes();
        turnController.EndPlayerTurn();
    }

    public void OnAttackMenuSelected()
    {
        if (!ValidateCommand()) return;

        Debug.Log($"{LOG_PREFIX} Attack Menu Selected");
        CancelAllModes();
    }

    public void OnNomalAttackSelected()
    {
        if (!ValidateCommand()) return;
        if (IsAnyAttackModeActive())
        {
            Debug.Log($"{LOG_PREFIX} 既に攻撃モード中");
            return;
        }

        Debug.Log($"{LOG_PREFIX} Normal Attack Selected");
        CancelAllModes();
        normalAttack.Execute();
    }

    public void OnSpecialAttackSelected()
    {
        if (!ValidateCommand()) return;
        if (IsAnyAttackModeActive())
        {
            Debug.Log($"{LOG_PREFIX} 既に攻撃モード中");
            return;
        }

        Debug.Log($"{LOG_PREFIX} Special Attack Selected");
        CancelAllModes();
        specialAttack.Execute();
    }

    public void OnSetTrapSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Trap Selected");
        CancelAllModes();
        setTrap.StartTrapMode();
    }

    #endregion

    #region Validation

    /// <summary>
    /// コマンド実行前の共通バリデーション
    /// </summary>
    private bool ValidateCommand()
    {
        if (!ValidatePlayerTurn()) return false;

        // トラップモード中は他のコマンドを受け付けない
        if (GuardTrapMode()) return false;

        return true;
    }

    private bool ValidatePlayerTurn()
    {
        if (turnController == null)
        {
            Debug.LogWarning($"{LOG_PREFIX} TurnController is null");
            return false;
        }

        if (!turnController.IsPlayerTurn)
        {
            Debug.Log($"{LOG_PREFIX} プレイヤーのターンではありません");
            return false;
        }

        return true;
    }

    /// <summary>
    /// トラップモード中かチェック、中断処理
    /// </summary>
    private bool GuardTrapMode()
    {
        if (setTrap != null && setTrap.IsTrapMode)
        {
            Debug.Log($"{LOG_PREFIX} TrapMode Cancelled by Other Command");
            setTrap.ForceCancel();
            return true;
        }
        return false;
    }

    #endregion

    #region Mode Management

    /// <summary>
    /// いずれかの攻撃モードがアクティブかチェック
    /// </summary>
    public bool IsAnyAttackModeActive()
    {
        bool normalActive = normalAttack != null && normalAttack.IsAttackMode();
        bool specialActive = specialAttack != null && specialAttack.IsAttackMode();
        return normalActive || specialActive;
    }

    /// <summary>
    /// いずれかのアクションモードがアクティブかチェック
    /// </summary>
    public bool IsAnyModeActive()
    {
        bool trapActive = setTrap != null && setTrap.IsTrapMode;
        return IsAnyAttackModeActive() || trapActive;
    }

    /// <summary>
    /// すべてのモードをキャンセル
    /// </summary>
    public void CancelAllModes()
    {
        if (playerMove != null)
        {
            playerMove.CancelMove();
        }

        if (normalAttack != null && normalAttack.IsAttackMode())
        {
            normalAttack.CancelAttack();
        }

        if (specialAttack != null && specialAttack.IsAttackMode())
        {
            specialAttack.CancelAttack();
        }

        if (setTrap != null && setTrap.IsTrapMode)
        {
            setTrap.ForceCancel();
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// 外部から現在のモード状態を取得
    /// </summary>
    public string GetCurrentModeStatus()
    {
        if (normalAttack != null && normalAttack.IsAttackMode())
            return "NormalAttack";

        if (specialAttack != null && specialAttack.IsAttackMode())
            return "SpecialAttack";

        if (setTrap != null && setTrap.IsTrapMode)
            return "Trap";

        return "None";
    }

    #endregion
}