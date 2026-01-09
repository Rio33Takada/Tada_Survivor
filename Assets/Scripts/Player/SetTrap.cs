using UnityEngine;
using System.Collections.Generic;

public class SetTrap : MonoBehaviour
{
    private const string LOG_PREFIX = "[SetTrap]";
    private const int TRAP_RANGE = 2;

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TurnController turnController;
    [SerializeField] private GameObject trapPrefab;

    [Header("Visual")]
    [SerializeField] private Color selectableTileColor = new Color(0.5f, 1f, 0.5f, 0.5f);

    private bool isTrapMode = false;
    private List<Vector2Int> selectablePositions = new();
    private HashSet<Vector2Int> occupiedPositions = new();
    private Dictionary<Vector2Int, Color> originalColors = new();

    public void StartTrapMode()
    {
        if (trapPrefab == null || gridManager == null) return;

        Debug.Log($"{LOG_PREFIX} Trap Mode Start");
        isTrapMode = true;

        CalculatePositions();
        HighlightTiles();

    }

    public void Tick()
    {
        if (!isTrapMode) return;

        if (Input.GetMouseButtonDown(0))
            TryPlaceTrap();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }

    private void CalculatePositions()
    {
        selectablePositions.Clear();
        Vector2Int playerPos = gridManager.WorldToGrid(transform.position);

        for (int x = -TRAP_RANGE; x <= TRAP_RANGE; x++)
            for (int y = -TRAP_RANGE; y <= TRAP_RANGE; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > TRAP_RANGE) continue;

                Vector2Int pos = playerPos + new Vector2Int(x, y);
                if (pos == playerPos) continue;
                if (occupiedPositions.Contains(pos)) continue;
                if (gridManager.IsWalkable(pos))
                    selectablePositions.Add(pos);
            }
    }

    private void HighlightTiles()
    {
        originalColors.Clear();
        foreach (var pos in selectablePositions)
        {
            originalColors[pos] = gridManager.GetTileColor(pos);
            gridManager.SetTileColor(pos, selectableTileColor);
        }
    }

    private void ResetTiles()
    {
        foreach (var kv in originalColors)
            gridManager.SetTileColor(kv.Key, kv.Value);

        originalColors.Clear();
    }

    private void TryPlaceTrap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Vector2Int gridPos = gridManager.WorldToGrid(hit.point);
        if (!selectablePositions.Contains(gridPos)) return;

        Vector3 worldPos = gridManager.GridToWorld(gridPos);
        Instantiate(trapPrefab, worldPos, Quaternion.identity);

        occupiedPositions.Add(gridPos);
        Debug.Log($"{LOG_PREFIX} Trap Placed {gridPos}");

        Cancel();
        if (turnController != null)
        {
            turnController.EndPlayerTurn();
        }
    }

    private void Cancel()
    {
        isTrapMode = false;
        ResetTiles();
        selectablePositions.Clear();
        Debug.Log($"{LOG_PREFIX} Trap Mode Cancel");
    }
}
