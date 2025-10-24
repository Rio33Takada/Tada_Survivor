using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uiManager;
    public GridManager gridManager;
    public TurnController turnController;

    [Header("Player")]
    public PlayerMove player; // PlayerMoveをInspectorで設定

    [Header("Game Stats")]
    public int AttackPoint { get; private set; } // 攻撃ポイント
    public int MovePoint { get; private set; }   // 移動ポイント
    public int WaveCount { get; private set; }   // 現在のウェーブ数

    private void Awake()
    {
        AwakeGame();
    }

    private void Start()
    {
        StartGame();
    }

    private void AwakeGame()
    {
        // グリッド生成
        if (gridManager != null)
            gridManager.GenerateGrid();
        else
            Debug.LogError("GridManager が設定されていません。");
    }

    private void StartGame()
    {
        InitializeVariables();

        if (uiManager != null)
            uiManager.InitializeUI(this);

        if (turnController != null)
            turnController.Initialize(this);

        if (player != null)
        {
            player.gridManager = gridManager;
            player.turnController = turnController; // ← 追加！
        }

        turnController.TurnChange();
    }


    private void InitializeVariables()
    {
        AttackPoint = 3;
        MovePoint = 5;
        WaveCount = 0;
    }

    public void NextWave()
    {
        WaveCount++;
        if (uiManager != null)
            uiManager.UpdateWave(WaveCount);
    }

    public void SpawnEnemy(int x, int y)
    {
        // 敵生成処理
        Debug.Log($"敵を生成: ({x}, {y})");
    }
}
