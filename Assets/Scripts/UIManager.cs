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
    private Canvas mainCanvas;

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
    [SerializeField] private Vector2 MoveButtonPos;
    [SerializeField] private Vector2 AttackButtonPos;
    [SerializeField] private Vector2 trapButtonPos;

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
        normalMove = CreateButton(MoveButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.EnableMoveOnce();
            commandController.OnMoveSelected();
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
