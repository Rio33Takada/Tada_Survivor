using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectButton : MonoBehaviour
{
    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.Find("BGM").GetComponent<SoundManager>();
    }
    public void SelectGrassland()
    {
        soundManager.PlaySE(SoundManager.SEType.Select2);
        SceneManager.LoadScene("Glassland_Map");
    }

    public void SelectVolcano()
    {
        soundManager.PlaySE(SoundManager.SEType.Select2);
        SceneManager.LoadScene("Volcano_Map");
    }
}
