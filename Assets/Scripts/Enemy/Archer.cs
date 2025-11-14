using takada;
using UnityEngine;

public class Archer : BattleEnemy
{
    public override int MaxHp => 1;

    public Archer() : base()
    {

    }

    public override void Move(Vector2Int playerPos, GridManager grid)
    {
        base.Move(playerPos, grid);
    }
}
