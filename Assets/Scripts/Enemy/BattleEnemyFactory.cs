using takada;
using UnityEngine;

public class BattleEnemyFactory
{
    private readonly EnemyPrefabHolder prefabHolder;
    private readonly BattleEnemyController enemyController;
    private readonly GridManager gridManager;

    public BattleEnemyFactory(
        EnemyPrefabHolder prefabHolder,
        BattleEnemyController enemyController,
        GridManager grid
        )
    {
        this.prefabHolder = prefabHolder;
        this.enemyController = enemyController;
        this.gridManager = grid;
    }

    public BattleEnemy CreateBattleEnemy(EnemyType type, Vector2Int pos)
    {
        var prefab = prefabHolder.GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogError($"Prefab が見つかりません: {type}");
            return null;
        }

        var go = GameObject.Instantiate(prefab);
        var enemy = go.GetComponent<BattleEnemy>();

        if (enemy == null)
        {
            Debug.LogError($"{type} のPrefab に BattleEnemy がアタッチされていません");
            return null;
        }

        enemy.SetPosition(pos, gridManager);

        enemyController.AddEnemy(enemy);

        return enemy;
    }
}