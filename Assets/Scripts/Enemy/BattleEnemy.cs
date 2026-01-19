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

namespace takada
{
    public abstract class BattleEnemy : MonoBehaviour
    {
        protected PlayerStatus player;
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
        public bool IsDead { get; internal set; }

        public event System.Action<BattleEnemy> OnDeath;

        protected virtual void Awake()
        {
            MaxHp = BaseMaxHp;
            Hp = MaxHp;
        }

        public void SetPlayerStatus(PlayerStatus status)
        {
            player = status;
        }

        public void SetPosition(Vector2Int pos, GridManager grid)
        {
            grid.GetTileAt(GridPosition).SetOccupantObject(null);
            GridPosition = pos;
            grid.GetTileAt(GridPosition).SetOccupantObject(gameObject);
            transform.position = new Vector3(pos.x, 0, pos.y);
        }

        private List<Vector2Int> GetPath(GridManager grid, Vector2Int playerPos)
        {
            Pathfinding pathfinder = new Pathfinding(grid);

            List<Vector2Int> goals = new List<Vector2Int>();

            foreach (var d in Dirs)
            {
                Vector2Int pos = playerPos + d;
                goals.Add(pos);
                if (GridPosition == pos) return null;
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

            if (bestPath == null || bestPath.Count < 2) return null;

            return bestPath;
        }

        public virtual async Task MoveAsync(Vector2Int playerPos, GridManager grid)
        {
            var bestPath = GetPath(grid, playerPos);
            if (bestPath == null) 
            {
                Vector3 lookDir = new Vector3(playerPos.x, 0, playerPos.y) - transform.position;

                await AnimationRotateAsync(lookDir);
                return;
            }

            Vector2Int nextPos = bestPath[1];

            Vector3 targetWorldPos = grid.GetTileAt(nextPos).transform.position;

            Vector3 moveDir = (targetWorldPos - transform.position);
            moveDir.y = 0f;
            await AnimationRotateAsync(moveDir);

            await AnimationMoveAsync(targetWorldPos);

            SetPosition(nextPos, grid);

            bestPath = GetPath(grid, playerPos);
            if (bestPath == null)
                moveDir = new Vector3(playerPos.x, 0, playerPos.y) - transform.position;
            else
            {
                targetWorldPos = grid.GetTileAt(nextPos).transform.position;
                moveDir = (targetWorldPos - transform.position);
            }

            await AnimationRotateAsync(moveDir);
        }

        public Task AnimationRotateAsync(Vector3 forward)
        {
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(RotateCoroutine(forward, tcs));
            return tcs.Task;
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

        private IEnumerator RotateCoroutine(Vector3 forward, TaskCompletionSource<bool> tcs)
        {
            if (forward == Vector3.zero)
            {
                tcs.SetResult(true);
                yield break;
            }

            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(forward);

            float duration = 0.15f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                t = Mathf.SmoothStep(0, 1, t);

                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            transform.rotation = targetRot;
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

            OnDeath?.Invoke( this );

            Destroy(gameObject);
        }

        public virtual void Attack(Vector2Int playerPos)
        {

        }
    }
}