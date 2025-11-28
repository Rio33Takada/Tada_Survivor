using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerMove playerMove;

    [SerializeField]
    private TurnController turnController;   // ★ 追加：ターン管理

    private CommandController commandController;
    private GameObject normalMove, normalAttack, trapAttack;

    [SerializeField]
<<<<<<< HEAD
    private Canvas mainCanvas;
=======
    private Canvas mainCanvas; // UI表示キャンバス.
    [SerializeField]
    private Transform buttonContainer; // コマンドボタン整列用オブジェクト.
>>>>>>> origin/feature/takada

    [SerializeField]
    private Text remainEnemyCountText,
                 waveCountText;

    [SerializeField]
<<<<<<< HEAD
    private GameObject attackPointIconsPrefab;
    private GameObject attackPointIcons;

    public GameObject buttonPrefab;
    private List<GameObject> buttons = new List<GameObject>();

    [Header("UI Positions")]
    [SerializeField] private Vector2 attackPointIconsPos;

    [Header("Command Button Positions")]
    [SerializeField] private Vector2 AttackButtonPos;
    [SerializeField] private Vector2 trapButtonPos;
    [SerializeField] private Vector2 EndButtonPos;
=======
    private GameObject attackPointIconsPrefab; // 攻撃ポイント表示オブジェクト(プレハブ).
    private GameObject attackPointIcons; // 攻撃ポイント表示オブジェクト(インスタンス).

    [SerializeField]
    private GameObject movePointCountDownPrefab; // 移動ポイントカウントダウン(プレハブ).
    private GameObject movePointCountDown; // 移動ポイントカウントダウン(インスタンス).

    void Start()
    {
        
    }
>>>>>>> origin/feature/takada

    void Update()
    {
        // ターンによってボタン制御
        if (turnController != null)
        {
            SetButtonsInteractable(turnController.IsPlayerTurn);
        }
    }

    private void ClassInitializer()
    {
        commandController = new CommandController();
    }

    public void InitializeUI(GameManager gm)
    {
        ClassInitializer();

        if (attackPointIcons != null)
            Destroy(attackPointIcons);

        attackPointIcons = Instantiate(attackPointIconsPrefab, mainCanvas.transform);

        RectTransform apRect = attackPointIcons.GetComponent<RectTransform>();
        apRect.anchoredPosition = attackPointIconsPos;

        SetSkillPoint(gm.AttackPoint);
        SetWaveCountText(gm.WaveCount);

        CreateCommandButton();
    }

    public void CreateCommandButton()
    {
<<<<<<< HEAD
        normalMove = CreateButton(EndButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;

            playerMove.CancelMove();          // 移動状態を解除
            turnController.EndPlayerTurn();   // ★ ターン終了
        });

        normalAttack = CreateButton(AttackButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnAttackSelected();
        });

        trapAttack = CreateButton(trapButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnSetTrapSelected();
        });
    }

    public void SetButtonsInteractable(bool canUse)
    {
        SetButtonState(normalMove.GetComponent<Button>(), canUse);
        SetButtonState(normalAttack.GetComponent<Button>(), canUse);
        SetButtonState(trapAttack.GetComponent<Button>(), canUse);
    }

    // ★ 敵ターンは完全透明
    private void SetButtonState(Button button, bool canUse)
    {
        button.interactable = canUse;

        Image img = button.GetComponent<Image>();
        Color c = img.color;

        c.a = canUse ? 1f : 0f;   // ← 敵ターンは消える

        img.color = c;
    }

    private GameObject CreateButton(Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject btn = Instantiate(buttonPrefab, mainCanvas.transform);
        buttons.Add(btn);

        RectTransform rect = btn.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        btn.GetComponent<Button>().onClick.AddListener(action);

        return btn;
=======
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
>>>>>>> origin/feature/takada
    }


    public void DeleteCommandButton()
    {
        foreach (var button in buttons)
            Destroy(button);

        buttons.Clear();
    }

    public void SetSkillPoint(int point)
    {
        attackPointIcons.GetComponent<AttackPointIconController>().AttackPointSet(point);
    }

    public void SetRemainEnemyCountText(int count)
    {
        remainEnemyCountText.text = "残り" + count + "体";
    }

    public void SetWaveCountText(int count)
    {
        waveCountText.text = count + "ウェーブ目";
    }

    public void UpdateWave(int wave)
    {
        SetWaveCountText(wave);
    }
}
