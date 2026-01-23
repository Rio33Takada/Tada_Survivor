using System.Threading.Tasks;
using takada;
using UnityEngine;

public class Knight : BattleEnemy
{
    protected override int BaseMaxHp => 1;

    public override async Task Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) 
            {
                Debug.Log("KnightはPlayerに攻撃した"); // プレイヤーにダメージ.
                PlayAttackAnimation();
                await WaitAttackAnimation();
                player.TakeDamage(1);
                return;
            }
        }
    }
}
