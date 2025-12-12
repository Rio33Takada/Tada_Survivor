using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectButton : MonoBehaviour
{
    public void SelectGrassland()
    {
        SceneManager.LoadScene("Glassland_Map");
    }

    public void SelectVolcano()
    {
        SceneManager.LoadScene("Volcano_Map");
    }
}
