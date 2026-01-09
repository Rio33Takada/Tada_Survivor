using takada;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
public class BattleEnemyController
{
    private BattleEnemyFactory enemyFactory;
    private GridManager gridManager;
    private PlayerMove player;

    public List<BattleEnemy> EnemyList { get; private set; }

    public BattleEnemyController(GridManager grid, EnemyPrefabHolder holder, PlayerMove player)
    {
        enemyFactory = new BattleEnemyFactory(holder, this, grid);
        gridManager = grid;
        this.player = player;

        EnemyList = new List<BattleEnemy>();
    }

    public void AddEnemy(BattleEnemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("enemy is null");
            return;
        }
        EnemyList.Add(enemy);
    }

    public void SpawnEnemy(EnemyType type, Vector2Int pos)
    {
        enemyFactory.CreateBattleEnemy(type, pos);
    }

    public async Task MoveEnemyAsync()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            await enemy.MoveAsync(player.gridPos, gridManager);
        }
    }


    public void AttackEnemy()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            enemy.Attack(player.gridPos);
        }
    }

    public void DamageEnemy(List<GameObject> enemies)
    {
        foreach(GameObject enemy in enemies)
        {
            var be = enemy.GetComponent<BattleEnemy>();
            EnemyList.Remove(be);
            be.TakeDamage(1);
        }
    }
}
