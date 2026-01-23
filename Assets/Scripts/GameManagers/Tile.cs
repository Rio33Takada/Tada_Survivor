using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;

    // ‰½‚à‚¢‚È‚¯‚ê‚Î•à‚¯‚é
    public bool Walkable => occupant == null;

    [Header("Occupant")]
    public GameObject occupant;

    [Header("Trap")]
    [SerializeField] private Trap trap;

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color moveColor = Color.cyan;
    [SerializeField] private Color attackRangeColor = new Color(1f, 0f, 0f, 0.4f);
    [SerializeField] private Color enemyAttackColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] private Color targetColor = Color.yellow;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = defaultColor;
    }

    public void SetMovableColor(bool enable)
    {
        rend.material.color = enable ? moveColor : defaultColor;
    }

    public void SetAttackRangeColor(bool enable)
    {
        rend.material.color = enable ? attackRangeColor : defaultColor;
    }

    public void SetEnemyAttackColor()
    {
        rend.material.color = enemyAttackColor;
    }

    public void SetTargetColor()
    {
        rend.material.color = targetColor;
    }

    public void ResetColor()
    {
        rend.material.color = defaultColor;
    }
    public void SetTrap(Trap t)
    {
        trap = t;
    }

    public void SetOccupantObject(GameObject obj)
    {
        occupant = obj;
        Debug.Log($"[Tile] occupant = {obj?.name}, trap = {trap}");

        if (trap != null && occupant != null)
        {
            var enemy = occupant.GetComponent<takada.BattleEnemy>();
            if (enemy != null)
            {
                Debug.Log("[Tile] Trap ”­“®");
                trap.OnStepped(enemy);
                trap = null;
            }
        }
    }


}
