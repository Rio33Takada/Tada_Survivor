using UnityEngine;
using UnityEngine.SceneManagement;

public class NextButton : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadScene("MapScene");
    }
}
