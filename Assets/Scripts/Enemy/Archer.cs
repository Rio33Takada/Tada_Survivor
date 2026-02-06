using System.Collections.Generic;
using System.Threading.Tasks;
using takada;
using UnityEngine;

public class Archer : BattleEnemy
{
    protected override int BaseMaxHp => 1;

    private ParticleSystem particle;

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

    public override async Task Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) 
            {
                Debug.Log("ArcherはPlayerに攻撃した"); // プレイヤーにダメージ.
                PlayAttackAnimation();
                await WaitAttackAnimation();
                GameObject.Find("BGM").GetComponent<SoundManager>().PlaySE(SoundManager.SEType.ArcherAttack);
                player.TakeDamage(1);
                return;
            }
        }
    }

    protected override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        particle.Play();
    }
}
