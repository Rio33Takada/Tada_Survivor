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

        Debug.LogError($"EnemyType {type} ‚Ì Prefab ‚ª“o˜^‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
        return null;
    }
}

namespace takada
{
    public abstract class BattleEnemy : MonoBehaviour
    {
        public virtual int MaxHp { get; }
        public int Hp { get; private set; }

        public Vector2Int GridPosition { get; private set; }

        public bool IsAlive => Hp > 0;

        public Vector2Int[] dirs =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        public BattleEnemy()
        {
            Hp = MaxHp;
        }

        public void SetPosition(Vector2Int pos)
        {
            GridPosition = pos;
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

            // --- ‡@ ƒS[ƒ‹Œó•âiƒvƒŒƒCƒ„[üˆÍ4ƒ}ƒXj‚ğæ“¾ ---
            List<Vector2Int> goals = new List<Vector2Int>();

            foreach (var d in dirs)
            {
                Vector2Int pos = playerPos + d;
                goals.Add(pos);
                if (GridPosition == new Vector2Int(pos.x, pos.y)) return;
            }

            // --- ‡A ŠeƒS[ƒ‹‚É‘Î‚µ‚Ä A* ‚ğÀs‚µ‚ÄÅ’ZŒo˜H‚ğ‘I‚Ô ---
            List<Vector2Int> bestPath = null;
            int bestCost = int.MaxValue;

            foreach (var g in goals)
            {
                List<Vector2Int> path = pathfinder.FindPath(GridPosition, g);

                if (path != null && path.Count < bestCost)
                {
                    bestCost = path.Count;
                    bestPath = path;
                }
            }

            // --- ‡B Œo˜H‚ªŒ©‚Â‚©‚ç‚È‚¢ ---
            if (bestPath == null || bestPath.Count < 2) return;


            // --- ‡C Å’ZŒo˜H‚ÉŠî‚Ã‚¢‚ÄˆÚ“® ---
            // bestPath[0] = Œ»İ’n, bestPath[1] = Ÿ‚Éi‚ŞˆÊ’u
            Vector2Int nextPos = bestPath[1];
            SetPosition(nextPos);

            // ƒ[ƒ‹ƒhÀ•W‚Ö”½‰f
            Tile nextTile = grid.GetTileAt(nextPos);
            if (nextTile != null)
            {
                transform.position = nextTile.transform.position;
            }
        }

        public virtual void Attack(Vector2Int playerPos)
        {

        }
    }
}