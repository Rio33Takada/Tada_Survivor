using takada;
using UnityEngine;

public class Knight : BattleEnemy
{
    protected override int BaseMaxHp => 1;

    public override void Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) Debug.Log("KnightはPlayerに攻撃した"); // プレイヤーにダメージ.
        }
    }
}
