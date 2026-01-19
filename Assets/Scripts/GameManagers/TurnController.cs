using UnityEngine;

public class TurnController : MonoBehaviour
{
    public bool IsPlayerTurn { get; private set; } = true;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private float enemyTurnDuration = 1.5f;

    private GameManager gameManager;
    private BattleEnemyController battleEnemyController;

    public void Initialize(GameManager manager, BattleEnemyController battleEnemyController)
    {
        gameManager = manager;
        this.battleEnemyController = battleEnemyController;
        StartPlayerTurn();
    }

    public void ClearAllButtons()
    {
        if (uiManager != null)
        {
            uiManager.ClearAllButtons();
        }
    }

    public void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;
        Debug.Log("プレイヤーターン終了 → 敵ターンへ");
        StartEnemyTurn();
    }

    private void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        uiManager.SetButtonsInteractable(true);
        if (uiManager != null)
        {
            uiManager.CreateMainCommandButtons(); 
        }
        Debug.Log("▶ プレイヤーターン開始");
    }

    private async void StartEnemyTurn()
    {
        IsPlayerTurn = false;
        uiManager.SetButtonsInteractable(false);
        Debug.Log("▶ 敵ターン開始");

        await battleEnemyController.MoveEnemyAsync();

        battleEnemyController.AttackEnemy();

        //StartCoroutine(EnemyTurnRoutine());

        // 敵の動作処理
        Invoke(nameof(EndEnemyTurn), enemyTurnDuration);
    }
    //private async Task<IEnumerator> EnemyTurnRoutine()
    //{
    //    // 移動
    //    await battleEnemyController.MoveEnemyAsync();

    //    // 攻撃
    //    battleEnemyController.AttackEnemy();

    //    yield return new WaitForSeconds(0.3f);

    //    EndEnemyTurn();
    //}

    private void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了 → プレイヤーターンへ");
        StartPlayerTurn();
    }
    private void GameOver()
    {
        Debug.Log("GameOver");

    }
}