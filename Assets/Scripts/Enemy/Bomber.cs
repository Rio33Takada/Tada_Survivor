using takada;
using UnityEngine;

public class Bomber : BattleEnemy
{
    public override int MaxHp => 1;

    public Bomber() : base()
    {

    }

    public override void Move(Vector2Int playerPos, GridManager grid)
    {
        base.Move(playerPos, grid);
    }

    public override void Attack(Vector2Int playerPos)
    {
        foreach (var d in dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) ; // プレイヤーにダメージ.
        }
    }
}
