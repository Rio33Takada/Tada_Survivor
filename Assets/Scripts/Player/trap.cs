using takada;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int skipTurn = 1; 


    public void OnStepped(BattleEnemy enemy)
    {
        if (enemy == null) return;

        enemy.SetSkipMove(skipTurn);
        Debug.Log($"トラップ発動！{skipTurn}ターン移動不可");

        Destroy(gameObject); // 使い捨て
    }
}
