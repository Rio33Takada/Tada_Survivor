using System.Collections.Generic;
using takada;
using UnityEngine;

public class Archer : BattleEnemy
{
    public override int MaxHp => 1;

    protected override Vector2Int[] Dirs { get; } =
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, 0),
        new Vector2Int(2, -1),
        new Vector2Int(1, -2),
        new Vector2Int(0, -2),
        new Vector2Int(-1, -2),
        new Vector2Int(-2, -1),
        new Vector2Int(-2, 0),
        new Vector2Int(-2, 1),
        new Vector2Int(-1, 2),
        new Vector2Int(0, 2),
        new Vector2Int(1, 2),
    };

    public override void Move(Vector2Int playerPos, GridManager gridManager)
    {
        GridManager grid = gridManager;
        Pathfinding pathfinder = new Pathfinding(grid);

        // --- ① ゴール候補（プレイヤー周囲4マス）を取得 ---
        List<Vector2Int> goals = new List<Vector2Int>();

        foreach (var d in Dirs)
        {
            Vector2Int pos = playerPos + d;
            goals.Add(pos);
            if (GridPosition == new Vector2Int(pos.x, pos.y)) return;
        }

        // --- ② 各ゴールに対して A* を実行して最短経路を選ぶ ---
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

        // --- ③ 経路が見つからない ---
        if (bestPath == null || bestPath.Count < 2) return;


        // --- ④ 最短経路に基づいて移動 ---
        // bestPath[0] = 現在地, bestPath[1] = 次に進む位置
        Vector2Int nextPos = bestPath[1];
        SetPosition(nextPos);

        // ワールド座標へ反映
        Tile nextTile = grid.GetTileAt(nextPos);
        if (nextTile != null)
        {
            transform.position = nextTile.transform.position;
        }
    }

    public override void Attack(Vector2Int playerPos)
    {
        foreach (var d in Dirs)
        {
            var pos = GridPosition + d;
            if (pos == playerPos) ; // プレイヤーにダメージ.
        }
    }
}
