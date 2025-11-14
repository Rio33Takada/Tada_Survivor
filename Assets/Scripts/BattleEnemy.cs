using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    knight,
    archer,
    bomber,
}

[CreateAssetMenu(fileName = "EnemyPrefabHolder", menuName = "Game/Enemy Prefab Holder")]
public class EnemyPrefabHolder : ScriptableObject
{
    [Serializable]
    public class EnemyPrefabPair
    {
        public EnemyType type;
        public GameObject prefab;
    }

    [SerializeField]
    private List<EnemyPrefabPair> enemyPrefabs = new List<EnemyPrefabPair>();

    private Dictionary<EnemyType, GameObject> prefabDictionary;

    public GameObject GetPrefab(EnemyType type)
    {
        if (prefabDictionary == null)
        {
            prefabDictionary = new Dictionary<EnemyType, GameObject>();
            foreach (var pair in enemyPrefabs)
            {
                if (!prefabDictionary.ContainsKey(pair.type))
                {
                    prefabDictionary.Add(pair.type, pair.prefab);
                }
            }
        }

        if (prefabDictionary.TryGetValue(type, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"EnemyType {type} の Prefab が登録されていません");
        return null;
    }
}

namespace takada
{
    public abstract class BattleEnemy : MonoBehaviour
    {
        public virtual int MaxHp { get; }
        public int Hp { get; private set; }

        public Vector2Int gridPosition;

        public bool IsAlive => Hp > 0;

        public BattleEnemy()
        {
            Hp = MaxHp;
        }

        public void TakeDamage(int amount)
        {
            Hp -= amount;
            if (!IsAlive)
            {
                Death();
            }
        }

        public void Death()
        {

        }

        public virtual void Move(Vector2Int playerPos, GridManager gridManager)
        {
            GridManager grid = gridManager;
            Pathfinding pathfinder = new Pathfinding(grid);

            List<Vector2Int> path = pathfinder.FindPath(gridPosition, playerPos);

            if (path == null || path.Count < 2) return;

            // path[0] = 今の位置, path[1] = 次の位置
            Vector2Int nextPos = path[1];

            gridPosition = nextPos;

            // 実際のワールド座標へ移動
            Tile tile = grid.GetTileAt(nextPos);
            if (tile != null)
            {
                transform.position = tile.transform.position;
            }
        }


        public virtual void Attack()
        {

        }
    }
}