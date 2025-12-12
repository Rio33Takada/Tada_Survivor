using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectButton : MonoBehaviour
{
    public void SelectGrassland()
    {
        SceneManager.LoadScene("player&grid");
    }

    public void SelectVolcano()
    {
        SceneManager.LoadScene("");
    }
}
