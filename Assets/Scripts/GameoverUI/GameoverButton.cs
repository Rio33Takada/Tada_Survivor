using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverButton : MonoBehaviour
{
    //ÉVÅ[ÉìÇÃñºëOäiî[
    public string[] sceneName =
    {
        "Volcano_Map",
        "Glassrand_Map"
    };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Retry()
    {
        SceneManager.LoadScene(sceneName[0]);
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
