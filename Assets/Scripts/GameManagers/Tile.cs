using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool walkable = true;
    public bool Walkable => occupant == null;

    // —áFŒã‚Å“G‚â”š’e‚ğ’u‚­‚Æ‚«‚Ég‚¤
    public GameObject occupant;
    private Renderer rend;
    private Color defaultColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public void SetMovableColor(bool movable)
    {
        rend.material.color = movable ? Color.cyan : defaultColor;
    }

    public void SetOccupantObject(GameObject gameObject)
    {
        occupant = gameObject;
    }
}
