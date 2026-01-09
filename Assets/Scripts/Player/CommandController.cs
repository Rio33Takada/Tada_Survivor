using UnityEngine;

public class CommandController : MonoBehaviour
{
    private const string LOG_PREFIX = "[CommandController]";

    [Header("References")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private TurnController turnController;

    [Header("Commands")]
    [SerializeField] private NormalAttack normalAttack;
    [SerializeField] private SpecialAttack specialAttack;
    [SerializeField] private SetTrap setTrap;

    private void Update()
    {
        // トラップモード中の入力処理は SetTrap に委譲
        if (setTrap != null)
        {
            setTrap.Tick();
        }
    }

    public void OnEndSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} End Selected");
        playerMove.CancelMove();
        turnController.EndPlayerTurn();
    }

    public void OnNomalAttackSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Normal Attack Selected");
        playerMove.CancelMove();
        normalAttack.Execute();
    }

    public void OnSpecialAttackSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Special Attack Selected");
        playerMove.CancelMove();
        specialAttack.Execute();
    }

    public void OnSetTrapSelected()
    {
        if (!ValidatePlayerTurn()) return;

        Debug.Log($"{LOG_PREFIX} Trap Selected");
        playerMove.CancelMove();
        setTrap.StartTrapMode();
    }

    private bool ValidatePlayerTurn()
    {
        if (turnController == null) return false;
        return turnController.IsPlayerTurn;
    }
}
