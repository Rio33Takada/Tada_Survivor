using takada;
using UnityEngine;
using System.Collections.Generic;
public class BattleEnemyController
{
    private BattleEnemyFactory enemyFactory;

    public List<BattleEnemy> enemyList { get; private set; }

    public BattleEnemyController()
    {
        enemyFactory = new BattleEnemyFactory(this);
    }

    public void AddEnemy(BattleEnemy enemy)
    {
        enemyList.Add(enemy);
    }

    public void MoveEnemy(Vector2Int playerPos)
    {
        foreach (BattleEnemy enemy in enemyList)
        {
            enemy.Move(playerPos);
        }
    }
}
