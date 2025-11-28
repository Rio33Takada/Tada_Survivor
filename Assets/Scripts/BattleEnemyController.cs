using takada;
using UnityEngine;
using System.Collections.Generic;
public class BattleEnemyController
{
    private BattleEnemyFactory enemyFactory;
    private GridManager gridManager;
    private PlayerMove player;

    public List<BattleEnemy> EnemyList { get; private set; }

    public BattleEnemyController(GridManager grid, EnemyPrefabHolder holder, PlayerMove player)
    {
        enemyFactory = new BattleEnemyFactory(holder, this);
        gridManager = grid;
        this.player = player;

        EnemyList = new List<BattleEnemy>();
    }

    public void AddEnemy(BattleEnemy enemy)
    {
        if (enemy == null) Debug.LogError("enemy is null");
        EnemyList.Add(enemy);
    }

    public void SpawnEnemy(EnemyType type, Vector2Int pos)
    {
        enemyFactory.CreateBattleEnemy(type, pos);
    }

    public void MoveEnemy()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            enemy.Move(player.gridPos, gridManager);
        }
    }

    public void AttackEnemy()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            enemy.Attack(player.gridPos);
        }
    }
}
