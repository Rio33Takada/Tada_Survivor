using takada;
using UnityEngine;

public class BattleEnemyFactory
{
    private readonly EnemyPrefabHolder prefabHolder;
    private readonly BattleEnemyController enemyController;

    public BattleEnemyFactory(
        EnemyPrefabHolder prefabHolder,
        BattleEnemyController enemyController)
    {
        this.prefabHolder = prefabHolder;
        this.enemyController = enemyController;
    }

    public void CreateBattleEnemy(EnemyType type, Vector2Int pos)
    {
        var prefab = prefabHolder.GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogError($"Prefab が見つかりません: {type}");
            return;
        }

        var go = GameObject.Instantiate(prefab);
        var enemy = go.GetComponent<BattleEnemy>();

        if (enemy == null)
        {
            Debug.LogError($"{type} のPrefab に BattleEnemy がアタッチされていません");
            return;
        }

        enemyController.AddEnemy(enemy);
    }
}