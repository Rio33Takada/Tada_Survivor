using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Retry()
    {
        SceneManager.LoadScene("");
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
