public class TurnController
{
    public bool IsPlayerTurn { get; private set; }

    public TurnController()
    {
        IsPlayerTurn = false;
    }

    public void TurnChange()
    {
        if (IsPlayerTurn)
        {
            EnemyTurn();
            IsPlayerTurn = false;
        }
        else
        {
            PlayerTurn();
            IsPlayerTurn = true;
        }
    }

    private void PlayerTurn()
    {

    }

    private void EnemyTurn()
    {

    }
}
