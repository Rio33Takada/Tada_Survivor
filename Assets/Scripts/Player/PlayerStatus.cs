using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("SP Settings")]
    [SerializeField] private int maxSP = 5;
    [SerializeField] private int currentSP;

    public int CurrentSP => currentSP;
    public int MaxSP => maxSP;

    private void Awake()
    {
        currentSP = maxSP;
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
        return true;
    }

    public void RecoverSP(int amount)
    {
        currentSP = Mathf.Min(maxSP, currentSP + amount);
        Debug.Log($"[PlayerStatus] SP‰ñ•œ: {amount} / Œ»ÝSP: {currentSP}");
    }
}
