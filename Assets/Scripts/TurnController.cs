public class TurnController
{
    public bool IsPlayerTurn { get; private set; }

    public TurnController()
    {
        IsPlayerTurn = true;
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
