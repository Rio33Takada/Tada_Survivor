using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearButton : MonoBehaviour
{
    public void OnButton()
    {
        SceneManager.LoadScene("");
    }
}
