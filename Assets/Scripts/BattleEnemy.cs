using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        protected virtual int BaseMaxHp => 1;
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }

        public Vector2Int GridPosition { get; private set; }

        public bool IsAlive => Hp > 0;

        protected virtual Vector2Int[] Dirs { get; } =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };

        protected virtual void Awake()
        {
            MaxHp = BaseMaxHp;
            Hp = MaxHp;
        }

        public void SetPosition(Vector2Int pos)
        {
            GridPosition = pos;
            transform.position = new Vector3(pos.x, 0, pos.y);
        }

        public virtual async Task MoveAsync(Vector2Int playerPos, GridManager gridManager)
        {
            GridManager grid = gridManager;
            Pathfinding pathfinder = new Pathfinding(grid);

            List<Vector2Int> goals = new List<Vector2Int>();

            foreach (var d in Dirs)
            {
                Vector2Int pos = playerPos + d;
                goals.Add(pos);
                if (GridPosition == pos) return;
            }

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

            if (bestPath == null || bestPath.Count < 2) return;

            Vector2Int nextPos = bestPath[1];

            Tile nextTile = grid.GetTileAt(nextPos);

            await AnimationMoveAsync(nextTile.transform.position);

            SetPosition(nextPos);
        }

        public Task AnimationMoveAsync(Vector3 targetPos)
        {
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(AnimationCoroutine(targetPos, tcs));
            return tcs.Task;
        }

        private IEnumerator AnimationCoroutine(Vector3 targetPos, TaskCompletionSource<bool> tcs)
        {
            float duration = 0.25f;
            float time = 0f;

            Vector3 startPos = transform.position;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                t = Mathf.SmoothStep(0, 1, t);

                transform.position = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
            }

            transform.position = targetPos;

            tcs.SetResult(true);
        }

        public void TakeDamage(int amount)
        {
            Hp -= amount;
            if (!IsAlive)
            {
                Death();
            }
        }

        public virtual void Death()
        {
            Debug.Log($"{this.name}‚ÍŽ€‚ñ‚¾");
        }

        public virtual void Attack(Vector2Int playerPos)
        {

        }
    }
}