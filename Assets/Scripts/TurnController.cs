using UnityEngine;

public class TurnController : MonoBehaviour
{
    public bool IsPlayerTurn { get; private set; } = true;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private float enemyTurnDuration = 1.5f;

    private GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        StartPlayerTurn();
    }

    public void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;

        Debug.Log("プレイヤーターン終了");
        StartEnemyTurn();
    }

    private void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        uiManager.SetButtonsInteractable(true);
        Debug.Log("▶ プレイヤーターン開始");
    }

    private void StartEnemyTurn()
    {
        IsPlayerTurn = false;
        uiManager.SetButtonsInteractable(false);
        Debug.Log("▶ 敵ターン開始");

        // 敵の動作処理
        Invoke(nameof(EndEnemyTurn), enemyTurnDuration);
    }

    private void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了 → プレイヤーターンへ");
        StartPlayerTurn();
    }
}