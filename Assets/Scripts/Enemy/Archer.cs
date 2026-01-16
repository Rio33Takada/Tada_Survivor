using System.Collections.Generic;
using System.Threading.Tasks;
using takada;
using UnityEngine;

public class Archer : BattleEnemy
{
    protected override int BaseMaxHp => 1;

    protected override Vector2Int[] Dirs { get; } =
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, 0),
        new Vector2Int(2, -1),
        new Vector2Int(1, -2),
        new Vector2Int(0, -2),
        new Vector2Int(-1, -2),
        new Vector2Int(-2, -1),
        new Vector2Int(-2, 0),
        new Vector2Int(-2, 1),
        new Vector2Int(-1, 2),
        new Vector2Int(0, 2),
        new Vector2Int(1, 2),
    };

    public override void Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) 
            {
                Debug.Log("ArcherはPlayerに攻撃した"); // プレイヤーにダメージ.
                player.TakeDamage(1);
            }
        }
    }
}
