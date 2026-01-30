using takada;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleEnemyController
{
    private readonly BattleEnemyFactory enemyFactory;
    private readonly GridManager gridManager;
    private readonly PlayerMove player;
    private readonly PlayerStatus playerStatus;
    private readonly UIManager uiManager;

    // 敵リスト（外部参照OKだが、削除はこのクラスのみ）
    public List<BattleEnemy> EnemyList { get; } = new();

    // =========================
    // Constructor
    // =========================
    public BattleEnemyController(
        GridManager grid,
        EnemyPrefabHolder holder,
        PlayerMove player,
        PlayerStatus status,
        UIManager ui)
    {
        gridManager = grid;
        this.player = player;
        playerStatus = status;
        uiManager = ui;

        enemyFactory = new BattleEnemyFactory(holder, this, gridManager);
    }

    // =========================
    // Enemy Management
    // =========================
    public void AddEnemy(BattleEnemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("[BattleEnemyController] enemy is null");
            return;
        }

        if (EnemyList.Contains(enemy)) return;

        EnemyList.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath;
    }

    public void SpawnEnemy(EnemyType type, Vector2Int pos)
    {
        var enemy = enemyFactory.CreateBattleEnemy(type, pos);
        if (enemy == null) return;

        enemy.SetPlayerStatus(playerStatus);
        AddEnemy(enemy);
    }

    // =========================
    // Enemy Turn Actions
    // =========================
    public async Task MoveEnemyAsync()
    {
        // ★ async中にListが変わっても安全
        var snapshot = EnemyList.ToArray();

        foreach (BattleEnemy enemy in snapshot)
        {
            if (enemy == null || enemy.IsDead) continue;

            await enemy.MoveAsync(player.gridPos, gridManager);
        }
    }

    public async Task AttackEnemyAsync()
    {
        var snapshot = EnemyList.ToArray();

        foreach (BattleEnemy enemy in snapshot)
        {
            if (enemy == null || enemy.IsDead) continue;

            await enemy.Attack(player.gridPos);
        }
    }

    // =========================
    // Damage Handling
    // =========================
    public void DamageEnemy(List<GameObject> enemies)
    {
        foreach (GameObject obj in enemies)
        {
            if (obj == null) continue;

            var enemy = obj.GetComponent<BattleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }
        // ★ EnemyListはここでは触らない（OnDeathに集約）
    }

    // =========================
    // Death Handling
    // =========================
    private void HandleEnemyDeath(BattleEnemy enemy)
    {
        if (enemy == null) return;

        enemy.OnDeath -= HandleEnemyDeath;

        if (EnemyList.Contains(enemy))
        {
            EnemyList.Remove(enemy);
        }

        // ★ 全滅チェックはここだけ
        if (EnemyList.Count <= 0)
        {
            Debug.Log("[BattleEnemyController] All enemies defeated");

            if (uiManager != null)
            {
                uiManager.OnGameClear();
            }
        }
    }
}
