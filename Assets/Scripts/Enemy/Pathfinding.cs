using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GridManager grid;

    public Pathfinding(GridManager grid)
    {
        this.grid = grid;
    }

    // A*で経路を返す
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start, null, 0, GetHeuristic(start, goal));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Fコストが最も低いノードを取得
            Node current = openList[0];
            foreach (var node in openList)
            {
                if (node.F < current.F) current = node;
            }

            // ゴール到達
            if (current.position == goal)
            {
                return ReconstructPath(current);
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector2Int nextPos in GetNeighbors(current.position))
            {
                Tile tile = grid.GetTileAt(nextPos);
                if (tile == null || !tile.Walkable) continue;

                Node neighbor = new Node(nextPos, current, current.G + 1, GetHeuristic(nextPos, goal));

                if (closedList.Contains(neighbor)) continue;

                // すでにOPENにある、かつ Gコストが高いなら更新しない
                Node openNode = openList.Find(n => n.Equals(neighbor));
                if (openNode == null)
                {
                    openList.Add(neighbor);
                }
                else if (neighbor.G < openNode.G)
                {
                    openNode.parent = current;
                    openNode.G = neighbor.G;
                }
            }
        }

        return null; // 経路なし
    }

    private List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // マンハッタン距離
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        yield return new Vector2Int(pos.x + 1, pos.y);
        yield return new Vector2Int(pos.x - 1, pos.y);
        yield return new Vector2Int(pos.x, pos.y + 1);
        yield return new Vector2Int(pos.x, pos.y - 1);
    }

    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int G; // 開始からのコスト
        public int H; // ヒューリスティック
        public int F => G + H;

        public Node(Vector2Int pos, Node parent, int g, int h)
        {
            this.position = pos;
            this.parent = parent;
            this.G = g;
            this.H = h;
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && node.position == position;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}