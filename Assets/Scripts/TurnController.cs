using UnityEngine;

public class TurnController : MonoBehaviour
{
    public bool IsPlayerTurn { get; private set; } = true;
    private GameManager gameManager;
    public UIManager uiManager;
    private BattleEnemyController battleEnemyController; // 11/28高田追加.

    public void Initialize(GameManager manager, BattleEnemyController enemyController)
    {
        gameManager = manager;
        battleEnemyController = enemyController; // 11/28高田追加.
        IsPlayerTurn = true;
    }

    public void TurnChange()
    {
        if (IsPlayerTurn)
        {
            // 敵ターンへ
            IsPlayerTurn = false;
            uiManager.SetButtonsInteractable(false); // ★ ボタン無効
            EnemyTurn();
        }
        else
        {
            // プレイヤーターンへ
            IsPlayerTurn = true;
            uiManager.SetButtonsInteractable(true); // ★ ボタン有効
            PlayerTurn();
        }
    }

    private void PlayerTurn()
    {
        IsPlayerTurn = true;
        Debug.Log("▶ プレイヤーターン開始");
    }

    private void EnemyTurn()
    {
        IsPlayerTurn = false;
        Debug.Log("▶ 敵ターン開始");

        // 例: 敵の動作をここに入れる
        battleEnemyController.MoveEnemy(); // 11/28高田追加.
        battleEnemyController.AttackEnemy(); // 11/28高田追加.


        // ここでは少し待ってからプレイヤーターンに戻す
        Invoke(nameof(EndEnemyTurn), 1.5f);
    }

    private void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了 → プレイヤーターンへ");
        TurnChange(); // プレイヤーへ戻す
    }

    public void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;

        Debug.Log("プレイヤーターン終了");
        IsPlayerTurn = false;

        // 敵ターンへ移行
        EnemyTurn();
    }
}
