using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[SpecialAttack]";
    private const int SP_COST = 1;

    [SerializeField] private PlayerStatus playerStatus;

    public void Execute()
    {
        if (playerStatus == null)
        {
            Debug.LogError($"{LOG_PREFIX} PlayerStatus 未設定");
            return;
        }

        // SPチェック＆消費
        if (!playerStatus.ConsumeSP(SP_COST))
        {
            Debug.Log($"{LOG_PREFIX} SP不足で特殊攻撃不可");
            return;
        }

        Debug.Log($"{LOG_PREFIX} 特殊攻撃（SP-{SP_COST}）");

        // 特殊攻撃の効果をここに
    }
}
