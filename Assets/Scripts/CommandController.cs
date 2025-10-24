using UnityEngine;

public class CommandController
{
    public CommandController()
    {

    }

    public void OnNormalAttackSelected()
    {
        Debug.Log("NormalAttackSelected");
    }

    public void OnSkillAttackSelected()
    {
        Debug.Log("SkillAttackSelected");
    }

    public void OnSetTrapSelected()
    {
        Debug.Log("SetTrapSelected");
    }
}
