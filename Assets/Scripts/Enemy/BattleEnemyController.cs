using takada;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleEnemyController
{
    private BattleEnemyFactory enemyFactory;
    private GridManager gridManager;
    private PlayerMove player;
    private PlayerStatus playerStatus;
    private UIManager uiManager;

    public List<BattleEnemy> EnemyList { get; private set; }

    // コンストラクタで UIManager を受け取る
    public BattleEnemyController(GridManager grid, EnemyPrefabHolder holder, PlayerMove player, PlayerStatus status, UIManager ui)
    {
        enemyFactory = new BattleEnemyFactory(holder, this, grid);
        gridManager = grid;
        this.player = player;
        playerStatus = status;
        uiManager = ui; // ここが重要

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

        enemy.OnDeath += HandleEnemyDeath;
    }

    public void SpawnEnemy(EnemyType type, Vector2Int pos)
    {
        var enemy = enemyFactory.CreateBattleEnemy(type, pos);
        enemy.SetPlayerStatus(playerStatus);
    }

    public async Task MoveEnemyAsync()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            await enemy.MoveAsync(player.gridPos, gridManager);
        }
    }

    public async Task AttackEnemy()
    {
        foreach (BattleEnemy enemy in EnemyList)
        {
            await enemy.Attack(player.gridPos);
        }
    }


    public void DamageEnemy(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            var be = enemy.GetComponent<BattleEnemy>();
            if (be != null)
            {
                be.TakeDamage(1);
            }
        }

        // 死亡済みの敵を EnemyList から削除
        EnemyList.RemoveAll(e => e.IsDead);

        // 敵が0になったらゲームクリア
        if (EnemyList.Count <= 0 && uiManager != null)
        {
            uiManager.OnGameClear();
        }
    }

    private void HandleEnemyDeath(BattleEnemy enemy)
    {
        EnemyList.Remove(enemy);

        // 敵が0になったらゲームクリア
        if (EnemyList.Count <= 0 && uiManager != null)
        {
            uiManager.OnGameClear();
        }
    }
}
