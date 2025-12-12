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

        public Vector2Int GridPosition { get; private set; }

        public bool IsAlive => Hp > 0;

        protected virtual Vector2Int[] Dirs { get; } =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };

        void Awake()
        {
            Hp = MaxHp;
        }

        public void SetPosition(Vector2Int pos)
        {
            GridPosition = pos;
            transform.position = new Vector3(pos.x, 0, pos.y);
        }

        public void AnimationMove(Vector3 targetPos)
        {
            StopAllCoroutines();              // 移動中なら中断
            StartCoroutine(MoveAnimation(targetPos));
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

            // ★ アニメーションを Task で待てるようにする
            await AnimationMoveAsync(nextTile.transform.position);

            // アニメ終了後に座標を更新
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

            // ★ アニメ完了
            tcs.SetResult(true);
        }

        private IEnumerator MoveAnimation(Vector3 targetPos)
        {
            float duration = 0.5f;           // 移動時間（好みで調整）
            float time = 0f;

            Vector3 startPos = transform.position;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                // イージング（お好みで変えられる）
                t = Mathf.SmoothStep(0, 1, t);

                transform.position = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
            }

            transform.position = targetPos;   // 最終位置を保証
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

        //public virtual void Move(Vector2Int playerPos, GridManager gridManager)
        //{
        //    GridManager grid = gridManager;
        //    Pathfinding pathfinder = new Pathfinding(grid);

        //    // --- ① ゴール候補（プレイヤー周囲4マス）を取得 ---
        //    List<Vector2Int> goals = new List<Vector2Int>();

        //    foreach (var d in Dirs)
        //    {
        //        Vector2Int pos = playerPos + d;
        //        goals.Add(pos);
        //        if (GridPosition == new Vector2Int(pos.x, pos.y)) return;
        //    }

        //    // --- ② 各ゴールに対して A* を実行して最短経路を選ぶ ---
        //    List<Vector2Int> bestPath = null;
        //    int bestCost = int.MaxValue;

        //    foreach (var g in goals)
        //    {
        //        List<Vector2Int> path = pathfinder.FindPath(GridPosition, g);

        //        if (path != null && path.Count < bestCost)
        //        {
        //            bestCost = path.Count;
        //            bestPath = path;
        //        }
        //    }

        //    // --- ③ 経路が見つからない ---
        //    if (bestPath == null || bestPath.Count < 2) return;


        //    // --- ④ 最短経路に基づいて移動 ---
        //    // bestPath[0] = 現在地, bestPath[1] = 次に進む位置
        //    Vector2Int nextPos = bestPath[1];
        //    SetPosition(nextPos);

        //    // ワールド座標へ反映
        //    Tile nextTile = grid.GetTileAt(nextPos);
        //    if (nextTile != null)
        //    {
        //        AnimationMove(nextTile.transform.position);
        //    }
        //}

        public virtual void Attack(Vector2Int playerPos)
        {

        }
    }
}