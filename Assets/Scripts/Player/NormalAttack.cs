using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    private const string LOG_PREFIX = "[NormalAttack]";

    public void Execute()
    {
        Debug.Log($"{LOG_PREFIX} Executing Normal Attack");

        // TODO:
        // ・攻撃範囲表示
        // ・敵選択
        // ・ダメージ計算
    }
}
