using takada;
using UnityEngine;
using System.Collections.Generic;
public class BattleEnemyController
{
    private BattleEnemyFactory enemyFactory;
    private GridManager gridManager;

    public List<BattleEnemy> enemyList { get; private set; }

    public BattleEnemyController(GridManager grid, EnemyPrefabHolder holder)
    {
        enemyFactory = new BattleEnemyFactory(holder, this);
        gridManager = grid;
    }

    public void AddEnemy(BattleEnemy enemy)
    {
        enemyList.Add(enemy);
    }

    public void MoveEnemy(Vector2Int playerPos)
    {
        foreach (BattleEnemy enemy in enemyList)
        {
            enemy.Move(playerPos, gridManager);
        }
    }
}
