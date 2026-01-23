using System.Threading.Tasks;
using takada;
using UnityEngine;

public class Bomber : BattleEnemy
{
    protected override int BaseMaxHp => 1;

    public override async Task Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) 
            {
                Debug.Log("BomberはPlayerに攻撃した"); // プレイヤーにダメージ.
                PlayAttackAnimation();
                await WaitAttackAnimation();
                player.TakeDamage(1);
                return;
            }
        }
    }

    public override void Death()
    {
        SetBomb();
        base.Death();
    }

    private void SetBomb()
    {
        Debug.Log($"{this.name}は爆弾を残した");
    }
}
