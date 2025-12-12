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

    private Button endButton;
    private Button attackButton;
    private Button trapButton;
    private Button normalAttackButton;
    private Button specialAttackButton;

    private void Update()
    {
        UpdateButtonStates();
        HandleEscapeInput();
    }

    public void InitializeUI(GameManager gameManager)
    {
        commandController = new CommandController();
        InitializeAttackPointIcons();
        SetSkillPoint(gameManager.AttackPoint);
        SetWaveCountText(gameManager.WaveCount);
        CreateMainCommandButtons();
    }

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
        }
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

    public void CreateMainCommandButtons()
    {
        ClearAllButtons();
        UnlockPlayerMovement();

        endButton = CreateCanvasButton(buttonPrefabs[0], endButtonPos, OnEndButtonClicked);
        attackButton = CreateCanvasButton(buttonPrefabs[1], attackButtonPos, OnAttackButtonClicked);
        trapButton = CreateCanvasButton(buttonPrefabs[2], trapButtonPos, OnTrapButtonClicked);

        CreateContainerButtons();
    }

    private void ShowAttackMenu()
    {
        ClearAllButtons();
        LockPlayerMovement();

        normalAttackButton = CreateCanvasButton(buttonPrefabs[3], normalAttackButtonPos,
            () => commandController.OnNomalAttackSelected());
        specialAttackButton = CreateCanvasButton(buttonPrefabs[4], specialAttackButtonPos,
            () => commandController.OnSpecialAttackSelected());
    }

    private void TrapSet()
    {

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
        {
            text.text = label;
        }

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(onClick);

        activeButtons.Add(buttonObj);
    }

    private Button CreateCanvasButton(GameObject prefab, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = Instantiate(prefab, mainCanvas.transform);
        activeButtons.Add(buttonObj);

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(action);

        return button;
    }

    private void ClearAllButtons()
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
        endButton = null;
        attackButton = null;
        trapButton = null;
        normalAttackButton = null;
        specialAttackButton = null;
    }

    private void OnEndButtonClicked()
    {
        if (!ValidatePlayerTurn()) return;

        playerMove.CancelMove();
        turnController.EndPlayerTurn();
        commandController.OnEndSelected();
    }

    private void OnAttackButtonClicked()
    {
        if (!ValidatePlayerTurn()) return;

        playerMove.CancelMove();
        ShowAttackMenu();
        commandController.OnAttackSelected();
    }

    private void OnTrapButtonClicked()
    {
        if (!ValidatePlayerTurn()) return;

        playerMove.CancelMove();
        TrapSet();
        commandController.OnSetTrapSelected();
    }

    private bool ValidatePlayerTurn()
    {
        return turnController != null && turnController.IsPlayerTurn;
    }

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

    public void SetButtonsInteractable(bool isInteractable)
    {
        SetButtonState(endButton, isInteractable);
        SetButtonState(attackButton, isInteractable);
        SetButtonState(trapButton, isInteractable);
        SetButtonState(normalAttackButton, isInteractable);
        SetButtonState(specialAttackButton, isInteractable);
    }

    private void SetButtonState(Button button, bool isInteractable)
    {
        if (button == null) return;

        button.interactable = isInteractable;

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = isInteractable ? 1f : 0f;
            image.color = color;
        }
    }

    public void SetSkillPoint(int point)
    {
        if (attackPointIcons != null)
        {
            AttackPointIconController controller = attackPointIcons.GetComponent<AttackPointIconController>();
            if (controller != null)
            {
                controller.AttackPointSet(point);
            }
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
}