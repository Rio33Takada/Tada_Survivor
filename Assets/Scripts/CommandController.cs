using UnityEngine;

public class CommandController
{
    public CommandController()
    {

    }

    public void OnMoveSelected()
    {
        Debug.Log("MoveSelected");
    }

    public void OnAttackSelected()
    {
        Debug.Log("AttackSelected");
    }

    public void OnSetTrapSelected()
    {
        Debug.Log("TrapSelected");
    }
}
