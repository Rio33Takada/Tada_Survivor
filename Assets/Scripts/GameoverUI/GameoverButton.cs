using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverButton : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Retry()
    {
        Scene Nscene = SceneManager.GetActiveScene();
        string sceneName = Nscene.name;
        SceneManager.LoadScene(sceneName);
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
