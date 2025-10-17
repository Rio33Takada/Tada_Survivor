using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    public int AttackPoint { get; private set; } // 攻撃ポイント.

    public int MovePoint { get; private set; } // 移動ポイント.

    public int WaveCount { get; private set; } // 現在のウェーブ数.

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        
    }

    private void StartGame()
    {
        // 変数初期化.
        InitializeVariables();

        // UI初期化.
        uiManager.InitializeUI(this);

        // フィールド生成.

        // プレイヤーターン開始.
    }

    private void InitializeVariables()
    {
        AttackPoint = 3;
        WaveCount = 0;
    }
}
