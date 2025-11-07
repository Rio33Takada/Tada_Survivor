using takada;
using UnityEngine;

public class Knight : BattleEnemy
{
    public override int MaxHp => 1;

    public Knight() : base()
    {

    }

    public override void Move(Vector2Int playerPos)
    {

    }

    public override bool SearchPlayer()
    {

        return false;
    }
}
