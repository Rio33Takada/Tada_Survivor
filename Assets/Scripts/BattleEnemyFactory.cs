using takada;
using UnityEngine;

public class BattleEnemyFactory
{
    private BattleEnemyController enemyController;

    public BattleEnemyFactory(BattleEnemyController enemyController)
    {
        this.enemyController = enemyController;
    }

    public void CreateBattleEnemy(EnemyType type)
    {
        BattleEnemy enemy = null;

        switch (type)
        {
            case EnemyType.knight:
                enemy = new Knight();
                break;
            default:
                Debug.LogError("ë∂ç›ÇµÇ»Ç¢EnemyTypeÇ≈Ç∑");
                break;
        }

        if (enemy != null)
        {
            enemyController.AddEnemy(enemy);
        }
    }
}
