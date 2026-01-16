using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("SP Settings")]
    [SerializeField] private int maxSP = 5;
    [SerializeField] private int currentSP;

    [Header("HP Settings")]
    [SerializeField] private int maxHP = 3;
    [SerializeField] private int currentHP;
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
            Debug.Log("[PlayerStatus] SP不足");
            return false;
        }

        currentSP -= amount;
        Debug.Log($"[PlayerStatus] SP消費: {amount} / 残りSP: {currentSP}");
        return true;
    }

    public void RecoverSP(int amount)
    {
        currentSP = Mathf.Min(maxSP, currentSP + amount);
        Debug.Log($"[PlayerStatus] SP回復: {amount} / 現在SP: {currentSP}");
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"現在HP{currentHP}");
        if (currentHP < 0)
        {
            Debug.Log("ゲームオーバー");
        }
    }
}
