using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private CommandController commandController;

    private List<GameObject> buttons = new List<GameObject>(); // ボタンリスト.

    public GameObject buttonPrefab; // ボタン(プレハブ).
    private GameObject normalAttack, skillAttack, trapAttack; // ボタンGameObject.

    [SerializeField]
    private Canvas mainCanvas; // UI表示キャンバス.
    [SerializeField]
    private Transform buttonContainer; // コマンドボタン整列用オブジェクト.

    [SerializeField]
    private Text remainEnemyCountText, // 残りの敵の数を表示するテキスト.
                 waveCountText;       // 現在のウェーブ数を表示するテキスト.

    [SerializeField]
    private GameObject attackPointIconsPrefab; // 攻撃ポイント表示オブジェクト(プレハブ).
    private GameObject attackPointIcons; // 攻撃ポイント表示オブジェクト(インスタンス).

    [SerializeField]
    private GameObject movePointCountDownPrefab; // 移動ポイントカウントダウン(プレハブ).
    private GameObject movePointCountDown; // 移動ポイントカウントダウン(インスタンス).

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void ClassInitializer()
    {
        commandController = new CommandController();
    }

    /// <summary>
    /// UI初期化
    /// </summary>
    public void InitializeUI(GameManager gm)
    {
        // クラス初期化.
        ClassInitializer();

        // 攻撃ポイント表示オブジェクト生成.
        if (attackPointIcons != null)
        {
            Destroy(attackPointIcons);
        }
        attackPointIcons = Instantiate(attackPointIconsPrefab);
        attackPointIcons.transform.SetParent(mainCanvas.transform);

        // 攻撃ポイント表示更新.
        SetSkillPoint(gm.AttackPoint);

        // 残り敵数表示更新.


        // 現在のウェーブ数表示更新.
        SetWaveCountText(gm.WaveCount);

        CreateCommandButton();
    }

    public void CreateCommandButton()
    {
        DeleteCommandButton();

        var buttonConfigs = new List<(string label, UnityEngine.Events.UnityAction onClick)>
        {
            ("通常攻撃", () => commandController.OnNormalAttackSelected()),
            ("スキル攻撃", () => commandController.OnSkillAttackSelected()),
            ("トラップ設置", () => commandController.OnSetTrapSelected())
        };

        foreach (var (label, onClick) in buttonConfigs)
        {
            var buttonObj = Instantiate(buttonPrefab, buttonContainer);
            var textComponent = buttonObj.GetComponentInChildren<Text>();
            if (textComponent != null)
                textComponent.text = label;

            buttonObj.GetComponent<Button>().onClick.AddListener(onClick);
            buttons.Add(buttonObj);
        }
    }


    public void DeleteCommandButton()
    {
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }

    public void SetSkillPoint(int point)
    {
        attackPointIcons.GetComponent<AttackPointIconController>().AttackPointSet(point);
    }

    public void SetRemainEnemyCountText(int count)
    {
        remainEnemyCountText.text = "残り" + count.ToString() + "体";
    }

    public void SetWaveCountText(int count)
    {
        waveCountText.text = count.ToString() + "ウェーブ目";
    }
    public void UpdateWave(int wave)
    {
        SetWaveCountText(wave);
    }
}
