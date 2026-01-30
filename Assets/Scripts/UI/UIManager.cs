using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private TurnController turnController;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Transform buttonContainer;

    [Header("UI Elements")]
    [SerializeField] private Text remainEnemyCountText;
    [SerializeField] private Text waveCountText;

    [Header("Prefabs")]
    [SerializeField] private List<GameObject> buttonPrefabs;
    [SerializeField] public GameObject GameoverUI;
    [SerializeField] public GameObject GameClearUI;

    [Header("Button Positions")]
    [SerializeField] private Vector2 attackButtonPos;
    [SerializeField] private Vector2 trapButtonPos;
    [SerializeField] private Vector2 endButtonPos;
    [SerializeField] private Vector2 normalAttackButtonPos;
    [SerializeField] private Vector2 specialAttackButtonPos;

    [Header("Icons")]
    [SerializeField] private AttackPointIconController attackPointIcons;
    [SerializeField] private AttackPointIconController hpIcons;

    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;

    private CommandController commandController;
    private readonly List<GameObject> activeButtons = new List<GameObject>();

    // ボタン参照
    private struct ButtonReferences
    {
        public Button End;
        public Button Attack;
        public Button Trap;
        public Button NormalAttack;
        public Button SpecialAttack;
    }
    private ButtonReferences buttons;

    // 定数
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
        SetSkillPoint(playerStatus.CurrentSP);
        SetHP(playerStatus.CurrentHP);
        SetWaveCountText(gameManager.WaveCount);
        CreateMainCommandButtons();
    }



    #endregion

    #region Input Handling

    private void UpdateButtonStates()
    {
        if (turnController == null || commandController == null) return;

        bool isPlayerTurn = turnController.IsPlayerTurn;
        bool isAnyModeActive = commandController.IsAnyModeActive();

        // プレイヤーターンかつ、いずれのモードもアクティブでない場合のみボタンを有効化
        bool shouldEnableButtons = isPlayerTurn && !isAnyModeActive;

        SetButtonsInteractable(shouldEnableButtons);
    }

    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (commandController != null)
            {
                commandController.CancelAllModes();
            }
            CreateMainCommandButtons();
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
            OnNormalAttackButtonClicked
        );

        buttons.SpecialAttack = CreateCanvasButton(
            buttonPrefabs[BUTTON_INDEX_SPECIAL_ATTACK],
            specialAttackButtonPos,
            OnSpecialAttackButtonClicked
        );
    }

    private void CreateContainerButtons()
    {
        CreateContainerButton(
            buttonPrefabs[BUTTON_INDEX_END],
            "移動",
            OnEndButtonClicked
        );

        CreateContainerButton(
            buttonPrefabs[BUTTON_INDEX_ATTACK],
            "攻撃",
            OnAttackButtonClicked
        );

        CreateContainerButton(
            buttonPrefabs[BUTTON_INDEX_TRAP],
            "トラップ",
            OnTrapButtonClicked
        );
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
        if (!CanExecuteCommand()) return;
        commandController.OnEndSelected();
    }

    private void OnAttackButtonClicked()
    {
        if (!CanExecuteCommand()) return;
        commandController.OnAttackMenuSelected();
        ShowAttackMenu();
    }

    private void OnTrapButtonClicked()
    {
        if (!CanExecuteCommand()) return;
        commandController.OnSetTrapSelected();
    }

    private void OnNormalAttackButtonClicked()
    {
        if (!CanExecuteCommand()) return;
        commandController.OnNomalAttackSelected();
    }

    private void OnSpecialAttackButtonClicked()
    {
        if (!CanExecuteCommand()) return;
        commandController.OnSpecialAttackSelected();
    }

    /// <summary>
    /// コマンド実行可能かチェック
    /// </summary>
    private bool CanExecuteCommand()
    {
        if (commandController == null)
        {
            Debug.LogWarning("[UIManager] CommandController is null");
            return false;
        }

        // 既にいずれかのモードがアクティブなら実行不可
        if (commandController.IsAnyModeActive())
        {
            Debug.Log("[UIManager] モード実行中のため新しいコマンドを受け付けません");
            return false;
        }

        return true;
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

    public void SetHP(int amount)
    {
        if (hpIcons != null)
        {
            hpIcons.AttackPointSet(amount);
        }
    }

    public void SetSkillPoint(int point)
    {
        if (attackPointIcons != null)
        {
            attackPointIcons.AttackPointSet(point);
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


    public void OnGameOver()
    {
        if (soundManager != null)
        {
            soundManager.PlayBGM(SoundManager.BGMType.GameOver);
        }
        if (GameoverUI != null)
        {
            Instantiate(GameoverUI);
        }
    }

    public void OnGameClear()
    {
        if (soundManager != null)
        {
            soundManager.PlayBGM(SoundManager.BGMType.GameClear);
        }
        if (GameClearUI != null)
        {
            Instantiate(GameClearUI);
        }
    }

    #endregion
}