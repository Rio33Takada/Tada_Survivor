using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Transform buttonContainer;

    [Header("UI Elements")]
    [SerializeField] private Text remainEnemyCountText;
    [SerializeField] private Text waveCountText;

    [Header("Prefabs")]
    [SerializeField] private GameObject attackPointIconsPrefab;
    [SerializeField] private List<GameObject> buttonPrefabs;
    [SerializeField] private GameObject movePointCountDownPrefab;

    [Header("Command Button Positions")]
    [SerializeField] private Vector2 AttackButtonPos;
    [SerializeField] private Vector2 trapButtonPos;
    [SerializeField] private Vector2 EndButtonPos;

    [Header("Icon Positions")]
    [SerializeField] private Vector2 attackPointIconsPos;

    private CommandController commandController;
    private GameObject attackPointIcons;
    private GameObject endButton, attackButton, trapButton;
    private List<GameObject> buttons = new List<GameObject>();

    void Update()
    {
        if (turnController != null)
        {
            SetButtonsInteractable(turnController.IsPlayerTurn);
        }
    }

    public void InitializeUI(GameManager gm)
    {
        commandController = new CommandController();

        InitializeAttackPointIcons();
        SetSkillPoint(gm.AttackPoint);
        SetWaveCountText(gm.WaveCount);
        CreateCommandButton();
    }

    private void InitializeAttackPointIcons()
    {
        if (attackPointIcons != null)
            Destroy(attackPointIcons);

        attackPointIcons = Instantiate(attackPointIconsPrefab, mainCanvas.transform);
        RectTransform apRect = attackPointIcons.GetComponent<RectTransform>();
        apRect.anchoredPosition = attackPointIconsPos;
    }

    public void CreateCommandButton()
    {
        DeleteCommandButton();

        endButton = CreateButton(buttonPrefabs[0], EndButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            turnController.EndPlayerTurn();
        });

        attackButton = CreateButton(buttonPrefabs[1], AttackButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnAttackSelected();
        });

        trapButton = CreateButton(buttonPrefabs[2], trapButtonPos, () =>
        {
            if (!turnController.IsPlayerTurn) return;
            playerMove.CancelMove();
            commandController.OnSetTrapSelected();
        });

        CreateContainerButtons();
    }

    private void CreateContainerButtons()
    {
        CreateContainerButton(buttonPrefabs[0], "移動", () => commandController.OnEndSelected());
        CreateContainerButton(buttonPrefabs[1], "攻撃", () => commandController.OnAttackSelected());
        CreateContainerButton(buttonPrefabs[2], "トラップ", () => commandController.OnSetTrapSelected());
    }

    private void CreateContainerButton(GameObject prefab, string label, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = Instantiate(prefab, buttonContainer);

        Text text = buttonObj.GetComponentInChildren<Text>();
        if (text != null)
            text.text = label;

        buttonObj.GetComponent<Button>().onClick.AddListener(onClick);
        buttons.Add(buttonObj);
    }

    private GameObject CreateButton(GameObject prefab, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject btn = Instantiate(prefab, mainCanvas.transform);
        buttons.Add(btn);

        // 親 Button の RectTransform を位置に合わせる
        RectTransform rect = btn.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        // ボタンのクリック処理
        Button buttonComp = btn.GetComponent<Button>();
        buttonComp.onClick.AddListener(action);

        // 親 Button で色変更や有効/無効を制御
        return btn;
    }


    public void DeleteCommandButton()
    {
        foreach (GameObject button in buttons)
        {
            if (button != null)
                Destroy(button);
        }

        buttons.Clear();
        endButton = null;
        attackButton = null;
        trapButton = null;
    }

    public void SetButtonsInteractable(bool canUse)
    {
        if (endButton != null)
            SetButtonState(endButton.GetComponent<Button>(), canUse);
        if (attackButton != null)
            SetButtonState(attackButton.GetComponent<Button>(), canUse);
        if (trapButton != null)
            SetButtonState(trapButton.GetComponent<Button>(), canUse);
    }

    private void SetButtonState(Button button, bool canUse)
    {
        if (button == null) return;

        button.interactable = canUse;

        Image img = button.GetComponent<Image>();
        if (img == null) return;

        Color c = img.color;
        c.a = canUse ? 1f : 0f;
        img.color = c;
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