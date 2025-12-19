using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TurnController turnController;
    [SerializeField] private CommandController commandController; // ← 追加

    [Header("Player")]
    [SerializeField] private PlayerMove player;

    [Header("Initial Stats")]
    [SerializeField] private int initialAttackPoint = 3;
    [SerializeField] private int initialMovePoint = 5;

    public int AttackPoint { get; private set; }
    public int MovePoint { get; private set; }
    public int WaveCount { get; private set; }

    BattleEnemyController battleEnemyController;
    [SerializeField] EnemyPrefabHolder enemyPrefabHolder;

    private void Awake()
    {
        ValidateReferences();
        InitializeGrid();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void ValidateReferences()
    {
        if (gridManager == null)
            Debug.LogError("GridManager が設定されていません。");
        if (uiManager == null)
            Debug.LogError("UIManager が設定されていません。");
        if (turnController == null)
            Debug.LogError("TurnController が設定されていません。");
        if (commandController == null)
            Debug.LogError("CommandController が設定されていません。"); 
        if (player == null)
            Debug.LogError("PlayerMove が設定されていません。");
    }

    private void InitializeGrid()
    {
        if (gridManager != null)
        {
            gridManager.GenerateGrid();
        }
    }

    private void InitializeGame()
    {
        InitializeStats();
        InitializeManagers();
        InitializePlayer();
        SpawnEnemy(6, 6);
        SpawnEnemy(4, 8);
    }

    private void InitializeStats()
    {
        RestorePoints();
        AttackPoint = initialAttackPoint;
        MovePoint = initialMovePoint;
        WaveCount = 0;
    }

    private void InitializeManagers()
    {
        battleEnemyController = new BattleEnemyController(gridManager, enemyPrefabHolder, player);

        if (uiManager != null)
        {
            uiManager.InitializeUI(this, commandController); // ← 修正！UI に CommandController を渡す
        }

        if (turnController != null)
        {
            turnController.Initialize(this, battleEnemyController);
        }
    }

    private void InitializePlayer()
    {
        if (player != null)
        {
            player.gridManager = gridManager;
            player.turnController = turnController;
        }
    }

    public void NextWave()
    {
        WaveCount++;

        if (uiManager != null)
        {
            uiManager.UpdateWave(WaveCount);
        }

        Debug.Log($"Wave {WaveCount} 開始");
    }

    public void SpawnEnemy(int x, int y)
    {
        Debug.Log($"敵を生成: ({x}, {y})");
        // TODO: 敵生成ロジックを実装
        EnemyType type = EnemyType.knight;
        switch (type)
        {
            case EnemyType.knight:
                battleEnemyController.SpawnEnemy(EnemyType.knight, new Vector2Int(x, y));
                break;
            case EnemyType.archer:
                battleEnemyController.SpawnEnemy(EnemyType.archer, new Vector2Int(x, y));
                break;
            case EnemyType.bomber:
                battleEnemyController.SpawnEnemy(EnemyType.bomber, new Vector2Int(x, y));
                break;
            default:
                return;
        }
    }

    public void ConsumeAttackPoint(int amount)
    {
        AttackPoint = Mathf.Max(0, AttackPoint - amount);

        if (uiManager != null)
        {
            //uiManager.SetSkillPoint(AttackPoint);
        }
    }

    public void ConsumeMovePoint(int amount)
    {
        MovePoint = Mathf.Max(0, MovePoint - amount);
    }

    public void RestorePoints()
    {
        AttackPoint = initialAttackPoint;
        MovePoint = initialMovePoint;

        if (uiManager != null)
        {
            //uiManager.SetSkillPoint(AttackPoint);
        }
    }
}
