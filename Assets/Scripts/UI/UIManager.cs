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

    [Header("Button Positions")]
    [SerializeField] private Vector2 attackButtonPos;
    [SerializeField] private Vector2 trapButtonPos;
    [SerializeField] private Vector2 endButtonPos;
    [SerializeField] private Vector2 normalAttackButtonPos;
    [SerializeField] private Vector2 specialAttackButtonPos;

    [Header("Icon Positions")]
    [SerializeField] private Vector2 attackPointIconsPos;

    private CommandController commandController;
    private GameObject attackPointIcons;
    private readonly List<GameObject> activeButtons = new List<GameObject>();

    // ボタン参照の構造体化
    private struct ButtonReferences
    {
        public Button End;
        public Button Attack;
        public Button Trap;
        public Button NormalAttack;
        public Button SpecialAttack;
    }
    private ButtonReferences buttons;

    // 定数定義
    private const int BUTTON_INDEX_END = 0;
    private const int BUTTON_INDEX_ATTACK = 1;
    private const int BUTTON_INDEX_TRAP = 2;
    private const int BUTTON_INDEX_NORMAL_ATTACK = 3;
    private const int BUTTON_INDEX_SPECIAL_ATTACK = 4;

    private const float BUTTON_ALPHA_VISIBLE = 1f;
    private const float BUTTON_ALPHA_HIDDEN = 0f;

    #region Unity Lifecycle

    private void Update()
    {
        UpdateButtonStates();
        HandleEscapeInput();
    }

    #endregion

    #region Initialization

    public void InitializeUI(GameManager gameManager, CommandController controller)
    {
        commandController = controller;
        InitializeAttackPointIcons();
        SetSkillPoint(gameManager.AttackPoint);
        SetWaveCountText(gameManager.WaveCount);
        CreateMainCommandButtons();
    }

    private void InitializeAttackPointIcons()
    {
        if (attackPointIcons != null)
        {
            Destroy(attackPointIcons);
        }

        attackPointIcons = Instantiate(attackPointIconsPrefab, mainCanvas.transform);
        RectTransform rectTransform = attackPointIcons.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = attackPointIconsPos;
    }

    #endregion

    #region Input Handling

    private void UpdateButtonStates()
    {
        if (turnController != null)
        {
            SetButtonsInteractable(turnController.IsPlayerTurn);
        }
    }

    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CreateMainCommandButtons();
            playerMove.CancelMove();
        }
    }

    #endregion

    #region Button Creation

    public void CreateMainCommandButtons()
    {
        ClearAllButtons();
        UnlockPlayerMovement();

        buttons.End = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_END],
            endButtonPos,
            OnEndButtonClicked
        );

        buttons.Attack = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_ATTACK],
            attackButtonPos,
            OnAttackButtonClicked
        );

        buttons.Trap = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_TRAP],
            trapButtonPos,
            OnTrapButtonClicked
        );

        CreateContainerButtons();
    }

    public void ShowAttackMenu()
    {
        ClearAllButtons();
        LockPlayerMovement();

        buttons.NormalAttack = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_NORMAL_ATTACK],
            normalAttackButtonPos,
            () => commandController.OnNomalAttackSelected()
        );

        buttons.SpecialAttack = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_SPECIAL_ATTACK],
            specialAttackButtonPos,
            () => commandController.OnSpecialAttackSelected()
        );
    }

    private void CreateContainerButtons()
    {
        CreateContainerButton(buttonPrefabs[BUTTON_INDEX_END], "移動",
            () => commandController.OnEndSelected());

        CreateContainerButton(buttonPrefabs[BUTTON_INDEX_ATTACK], "攻撃",
            () => commandController.OnNomalAttackSelected());

        CreateContainerButton(buttonPrefabs[BUTTON_INDEX_TRAP], "トラップ",
            () => commandController.OnSetTrapSelected());
    }

    private void CreateContainerButton(GameObject prefab, string label, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = Instantiate(prefab, buttonContainer);

        Text text = buttonObj.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = label;
        }

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(onClick);
        }

        activeButtons.Add(buttonObj);
    }

    private Button CreateCanvasButton(GameObject prefab, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = Instantiate(prefab, mainCanvas.transform);
        activeButtons.Add(buttonObj);

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(action);
        }

        return button;
    }

    public void ClearAllButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }

        activeButtons.Clear();
        ResetButtonReferences();
    }

    private void ResetButtonReferences()
    {
        buttons = new ButtonReferences();
    }

    #endregion

    #region Button Click Handlers

    private void OnEndButtonClicked()
    {
        commandController.OnEndSelected();
    }

    private void OnAttackButtonClicked()
    {
        commandController.OnAttackMenuSelected();
        ShowAttackMenu();
    }

    private void OnTrapButtonClicked()
    {
        commandController.OnSetTrapSelected();
    }

    #endregion

    #region Player Movement Control

    private void LockPlayerMovement()
    {
        if (playerMove != null)
        {
            playerMove.isActionLocked = true;
        }
    }

    private void UnlockPlayerMovement()
    {
        if (playerMove != null)
        {
            playerMove.isActionLocked = false;
        }
    }

    #endregion

    #region Button State Management

    public void SetButtonsInteractable(bool isInteractable)
    {
        SetButtonState(buttons.End, isInteractable);
        SetButtonState(buttons.Attack, isInteractable);
        SetButtonState(buttons.Trap, isInteractable);
        SetButtonState(buttons.NormalAttack, isInteractable);
        SetButtonState(buttons.SpecialAttack, isInteractable);
    }

    private void SetButtonState(Button button, bool isInteractable)
    {
        if (button == null) return;

        button.interactable = isInteractable;

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = isInteractable ? BUTTON_ALPHA_VISIBLE : BUTTON_ALPHA_HIDDEN;
            image.color = color;
        }
    }

    #endregion

    #region UI Updates

    public void SetSkillPoint(int point)
    {
        if (attackPointIcons == null) return;

        AttackPointIconController controller = attackPointIcons.GetComponent<AttackPointIconController>();
        if (controller != null)
        {
            controller.AttackPointSet(point);
        }
    }

    public void SetRemainEnemyCountText(int count)
    {
        if (remainEnemyCountText != null)
        {
            remainEnemyCountText.text = $"残り{count}体";
        }
    }

    public void SetWaveCountText(int count)
    {
        if (waveCountText != null)
        {
            waveCountText.text = $"{count}ウェーブ目";
        }
    }

    public void UpdateWave(int wave)
    {
        SetWaveCountText(wave);
    }

    #endregion
}