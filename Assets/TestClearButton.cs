using UnityEngine;

public class TestClearButton : MonoBehaviour
{
    public GameObject obj;

    public void OnC()
    {
        Instantiate(obj);
    }

    public void OnG()
    {
        Instantiate(obj);
    }
}
