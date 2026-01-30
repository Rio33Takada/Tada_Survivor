using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("SP Settings")]
    [SerializeField] private int maxSP = 5;
    [SerializeField] private int currentSP;

    [Header("HP Settings")]
    [SerializeField] private int maxHP = 3;
    [SerializeField] private int currentHP;

    [SerializeField] private UIManager uiManager;

    public int CurrentSP => currentSP;
    public int MaxSP => maxSP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentSP = maxSP;
        currentHP = maxHP;
    }

    public bool ConsumeSP(int amount)
    {
        if (currentSP < amount)
        {
            Debug.Log("[PlayerStatus] SP•s‘«");
            return false;
        }

        currentSP -= amount;
        Debug.Log($"[PlayerStatus] SPÁ”ï: {amount} / Žc‚èSP: {currentSP}");

        if (uiManager != null)
        {
            uiManager.SetSkillPoint(currentSP); 
        }

        return true;
    }

    public void RestoreSP(int amount)
    {
        currentSP = Mathf.Clamp(currentSP + amount, 0, maxSP);
        Debug.Log($"[PlayerStatus] SP‰ñ•œ: +{amount} ({currentSP}/{maxSP})");

        if (uiManager != null)
        {
            uiManager.SetSkillPoint(currentSP);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"Œ»ÝHP{currentHP}");
        uiManager.SetHP(currentHP);
        if (currentHP <= 0)
        {
            uiManager.OnGameOver();
        }
    }
}
