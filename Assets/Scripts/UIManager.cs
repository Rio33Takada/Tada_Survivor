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
    private Canvas mainCanvas; // UI表示キャンバス.
    [SerializeField]
    private Transform buttonContainer; // コマンドボタン整列用オブジェクト.

    [SerializeField]
    private Text remainEnemyCountText,
                 waveCountText;

    [SerializeField]
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

    [SerializeField]
    private GameObject movePointCountDownPrefab; // 移動ポイントカウントダウン(プレハブ).


    void Update()
    {
        // ★ 修正: turnController (小文字) を使用
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
        // ★ 修正: 既存のボタンを削除してから作成
        DeleteCommandButton();

        // ターン終了ボタン
        normalMove = CreateButton(EndButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();          // 移動状態を解除
            turnController.EndPlayerTurn();   // ★ ターン終了
        });

        // 通常攻撃ボタン
        normalAttack = CreateButton(AttackButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnAttackSelected();
        });

        // トラップ設置ボタン
        trapAttack = CreateButton(trapButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnSetTrapSelected();
        });

        // ★ 修正: buttonContainer用のボタンを作成（元のコードから移動）
        CreateContainerButtons();
    }

    // ★ 新規追加: buttonContainer用のボタンを別メソッドに分離
    private void CreateContainerButtons()
    {
        // ★ 修正: 明示的にUnityActionにキャスト
        var buttonConfigs = new List<(string label, UnityEngine.Events.UnityAction onClick)>
        {
            ("通常攻撃", (UnityEngine.Events.UnityAction)(() => commandController.OnMoveSelected())),
            ("スキル攻撃", (UnityEngine.Events.UnityAction)(() => commandController.OnAttackSelected())),
            ("トラップ設置", (UnityEngine.Events.UnityAction)(() => commandController.OnSetTrapSelected()))
        };

        foreach (var config in buttonConfigs)
        {
            var buttonObj = Instantiate(buttonPrefab, buttonContainer);
            var textComponent = buttonObj.GetComponentInChildren<Text>();
            if (textComponent != null)
                textComponent.text = config.label;

            buttonObj.GetComponent<Button>().onClick.AddListener(config.onClick);
            buttons.Add(buttonObj);
        }
    }

    public void SetButtonsInteractable(bool canUse)
    {
        if (normalMove != null)
            SetButtonState(normalMove.GetComponent<Button>(), canUse);
        if (normalAttack != null)
            SetButtonState(normalAttack.GetComponent<Button>(), canUse);
        if (trapAttack != null)
            SetButtonState(trapAttack.GetComponent<Button>(), canUse);
    }

    // ★ 敵ターンは完全透明
    private void SetButtonState(Button button, bool canUse)
    {
        if (button == null) return;

        button.interactable = canUse;

        Image img = button.GetComponent<Image>();
        if (img == null) return;

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
    }

    public void DeleteCommandButton()
    {
        foreach (var button in buttons)
        {
            if (button != null)
                Destroy(button);
        }

        buttons.Clear();

        // メンバ変数もクリア
        normalMove = null;
        normalAttack = null;
        trapAttack = null;
    }

    public void SetSkillPoint(int point)
    {
        if (attackPointIcons != null)
            attackPointIcons.GetComponent<AttackPointIconController>().AttackPointSet(point);
    }

    public void SetRemainEnemyCountText(int count)
    {
        if (remainEnemyCountText != null)
            remainEnemyCountText.text = "残り" + count + "体";
    }

    public void SetWaveCountText(int count)
    {
        if (waveCountText != null)
            waveCountText.text = count + "ウェーブ目";
    }

    public void UpdateWave(int wave)
    {
        SetWaveCountText(wave);
    }
}