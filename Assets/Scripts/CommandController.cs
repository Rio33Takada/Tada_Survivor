using UnityEngine;

public class CommandController : MonoBehaviour
{
    public void OnEndSelected()
    {
        Debug.Log("EndSelected");
    }

    public void OnAttackSelected()
    {
        Debug.Log("AttackMenuSelected");
    }
    public void OnNomalAttackSelected()
    {
        Debug.Log("NomalAttackSelected");
    }

    public void OnSpecialAttackSelected()
    {
        Debug.Log("SpecialAttackSelected");
    }

    public void OnSetTrapSelected()
    {
        Debug.Log("TrapSelected");
    }
}
